using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public DepartmentController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            }
        }
        // GET: api/Department
        [HttpGet]
        public IEnumerable<Department> Get(string _include, string _filter, string _gt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "employees")
                    {
                        cmd.CommandText = @"SELECT d.Id as DepartmentId, 
                                            d.Name as DepartmentName, 
                                            d.Budget as DepartmentBudget,
                                            e.Id as EmployeeId,
                                            e.IsSupervisor as IsSupervisor,
                                            e.FirstName as EmployeeFirstName,
                                            e.LastName as EmployeeLastName
                                            FROM Department as d 
                                            left join Employee as e on d.Id = e.DepartmentId";
                    }
                    else if (_filter == "budget" && _gt=="300000") 
                    {
                        cmd.CommandText = @"SELECT Budget as DepartmentBudget, Id as DepartmentId, Name as DepartmentName
                                            FROM Department
                                            WHERE Budget >= 300000 ";
                    } 
                    else
                    {
                        cmd.CommandText = @"SELECT Id as DepartmentId, Name as DepartmentName, Budget as DepartmentBudget FROM Department";
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Department> departments = new Dictionary<int, Department>();
                    while(reader.Read())
                    {
                        int DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                        if (!departments.ContainsKey(DepartmentId))
                        {
                            Department newDepartment = new Department
                            {
                                Id = DepartmentId,
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))
                            };
                            departments.Add(DepartmentId, newDepartment);
                        } 
                        if (_include == "employees")
                        {
                            if(!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                            {
                                Department currentDepartment = departments[DepartmentId];
                                currentDepartment.EmployeeList.Add(
                                    new Employee
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                                        IsSupervisor = reader.GetBoolean((reader.GetOrdinal("IsSupervisor")))
                                    }
                                    );
                            }
                        }
                    }
                        reader.Close();
                        return departments.Values.ToList();
                }
            }
        }

        // GET: api/Department/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id, string _include, string _filter, string _gt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "employees")
                    {
                        cmd.CommandText = @"SELECT d.Id as DepartmentId, 
                                            d.Name as DepartmentName, 
                                            d.Budget as DepartmentBudget,
                                            e.Id as EmployeeId,
                                            e.IsSupervisor as IsSupervisor,
                                            e.FirstName as EmployeeFirstName,
                                            e.LastName as EmployeeLastName
                                            FROM Department as d 
                                            left join Employee as e on d.Id = e.DepartmentId
                                            WHERE d.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                    }
                    else
                    {
                        cmd.CommandText = @"SELECT Id as DepartmentId, Name as DepartmentName, Budget as DepartmentBudget FROM Department WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = null;
                    while(reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))

                        };

                        if(_include == "employees")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                            {
                                if(!department.EmployeeList.Exists(x =>x.Id == reader.GetInt32(reader.GetOrdinal("EmployeeId"))))
                                {
                                    department.EmployeeList.Add(
                                         new Employee
                                         {
                                             Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                             FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                                             LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                                             IsSupervisor = reader.GetBoolean((reader.GetOrdinal("IsSupervisor")))
                                         }
                                        );
                                }                                    
                            }
                        }
                    }
                    reader.Close();
                    return Ok(department);
                }
            }
        }

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department newDepartment)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO  Department (Name, Budget)
                                         OUTPUT INSERTED.Id
                                         Values(@Name, @Budget)";
                    cmd.Parameters.Add(new SqlParameter("@Name", newDepartment.Name));
                    cmd.Parameters.Add(new SqlParameter("@Budget", newDepartment.Budget));

                    int newId = (int)cmd.ExecuteScalar();
                    newDepartment.Id = newId;
                    return CreatedAtRoute("GetDepartment", new { id = newId }, newDepartment);

                }
            }
        }

        // PUT: api/Department/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Department department)
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Department 
                                        SET Name = @Name, Budget = @Budget
                                        WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return NoContent();
                }
            }

        }

    }

}

