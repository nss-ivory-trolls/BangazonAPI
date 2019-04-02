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
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Employee
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
