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
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public EmployeeController(IConfiguration configuration)
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
        // GET: api/Employee
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        e.Id as EmployeeId,
                                        e.FirstName as EmployeeFirstName,
                                        e.IsSupervisor as IsSupervisor,
                                        e.LastName as EmployeeLastName, 
                                        d.Id as DepartmentId,
                                        d.Name as DepartmentName,
                                        c.Id as ComputerId,
                                        c.Make as ComputerMake,
                                        c.Manufacturer as ComputerManufacturer, 
                                        c.PurchaseDate as ComputerPurchaseDate,
                                        c.DecomissionDate as ComputerDecomissionDate
                                        from Employee as e
                                        left join department as d on e.DepartmentId = d.Id
                                        left join ComputerEmployee as ce on e.Id = ce.EmployeeId
                                        left join Computer as c on ce.ComputerId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();
                    while(reader.Read())
                    {
                        Employee newEmployee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))

                            },
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("ComputerPurchaseDate")),
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("ComputerDecomissionDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("ComputerDecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerMake")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("ComputerManufacturer"))
                            }

                        };
                        employees.Add(newEmployee);
                    }
                    reader.Close();
                    return employees;

                }
            }
        }

        // GET: api/Employee/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        e.Id as EmployeeId,
                                        e.FirstName as EmployeeFirstName,
                                        e.IsSupervisor as IsSupervisor,
                                        e.LastName as EmployeeLastName, 
                                        d.Id as DepartmentId,
                                        d.Name as DepartmentName,
                                        c.Id as ComputerId,
                                        c.Make as ComputerMake,
                                        c.Manufacturer as ComputerManufacturer, 
                                        c.PurchaseDate as ComputerPurchaseDate,
                                        c.DecomissionDate as ComputerDecomissionDate
                                        from Employee as e
                                        left join department as d on e.DepartmentId = d.Id
                                        left join ComputerEmployee as ce on e.Id = ce.EmployeeId
                                        left join Computer as c on ce.ComputerId = c.Id
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;
                    while(reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))

                            },
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("ComputerPurchaseDate")),
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("ComputerDecomissionDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("ComputerDecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerMake")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("ComputerManufacturer"))
                            }

                        };
                    }
                    reader.Close();
                    return Ok(employee);
                }
            }
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO  Employee (FirstName, LastName, DepartmentId, IsSupervisor)
                                         OUTPUT INSERTED.Id
                                         Values(@FirstName, @LastName, @DepartmentId, @IsSupervisor)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", newEmployee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", newEmployee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", newEmployee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@IsSupervisor", newEmployee.IsSupervisor));

                    int newId = (int)cmd.ExecuteScalar();
                    newEmployee.Id = newId;
                    return CreatedAtRoute("GetEmployee", new { id = newId }, newEmployee);

                }
            }
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Employee employee)
        {

            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Employee 
                                        SET FirstName = @FirstName, LastName = @LastName, DepartmentId=@DepartmentId, IsSupervisor=@IsSupervisor
                                        WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@IsSupervisor", employee.IsSupervisor)); 
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return NoContent();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
