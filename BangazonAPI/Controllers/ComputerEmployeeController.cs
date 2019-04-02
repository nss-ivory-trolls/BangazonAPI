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
    public class ComputerEmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerEmployeeController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        //Get all employees' computers
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT ce.Id, ce.AssignDate, ce.UnassignDate, ce.EmployeeId, ce.ComputerId, 
                                        e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor,
                                        c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM ComputerEmployee ce
                                        LEFT JOIN Employee e ON ce.EmployeeId = e.Id
                                        LEFT JOIN Computer c ON ce.ComputerId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<ComputerEmployee> employeeComputerList = new List<ComputerEmployee>();

                    while (reader.Read())
                    {
                        ComputerEmployee employeeComputer = new ComputerEmployee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AssignDate = reader.GetDateTime(reader.GetOrdinal("AssignDate")),
                            UnassignDate = reader.IsDBNull(reader.GetOrdinal("UnassignDate")) ? (DateTime?) null : (DateTime?) reader.GetDateTime(reader.GetOrdinal("UnassignDate")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            Employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            },
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            }
                        };
                        employeeComputerList.Add(employeeComputer);
                    }
                    reader.Close();
                    return Ok(employeeComputerList);
                }
            }
        }

        //Get an employee's computer by Id
        [HttpGet("{id}", Name = "GetEmployeeComputerById")]
        public async Task<IActionResult> GetEmployeeComputerById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT ce.Id, ce.AssignDate, ce.UnassignDate, ce.EmployeeId, ce.ComputerId, 
                                        e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor,
                                        c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM ComputerEmployee ce
                                        LEFT JOIN Employee e ON ce.EmployeeId = e.Id
                                        LEFT JOIN Computer c ON ce.ComputerId = c.Id
                                        WHERE ce.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    ComputerEmployee employeeComputer = null;

                    while (reader.Read())
                    {
                        employeeComputer = new ComputerEmployee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AssignDate = reader.GetDateTime(reader.GetOrdinal("AssignDate")),
                            UnassignDate = reader.IsDBNull(reader.GetOrdinal("UnassignDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("UnassignDate")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            Employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            },
                            ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            }
                        };
                    }
                    reader.Close();
                    return Ok(employeeComputer);
                }
            }
        }

        //Assign a computer to an employee
        [HttpPost]
        public async Task<IActionResult> PostEmployeeComputer([FromBody] ComputerEmployee employeeComputer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ComputerEmployee (AssignDate, EmployeeId, ComputerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@assignDate, @employeeId, @computerId)";
                    cmd.Parameters.Add(new SqlParameter("@assignDate", employeeComputer.AssignDate));
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeComputer.EmployeeId));
                    cmd.Parameters.Add(new SqlParameter("@computerId", employeeComputer.ComputerId));

                    int newId = (int)cmd.ExecuteScalar();
                    employeeComputer.Id = newId;
                    return CreatedAtRoute("GetEmployeeComputerById", new { id = newId }, employeeComputer);
                }
            }
        }

        //Edit an employees' computer
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeComputer([FromRoute] int id, [FromBody] ComputerEmployee employeeComputer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE ComputerEmployee
                                        SET AssignDate = @assignDate,
                                            EmployeeId = @employeeId,
                                            ComputerId = @computerId
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@assignDate", employeeComputer.AssignDate));
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeComputer.EmployeeId));
                    cmd.Parameters.Add(new SqlParameter("@computerId", employeeComputer.ComputerId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return NoContent();
                    }
                    throw new Exception("No rows affected");
                }
            }
        }

        //Delete an employee's computer entry
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeComputer([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ComputerEmployee
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return NoContent();
                    }
                    throw new Exception("No rows affected");
                }
            }
        }
    }
}