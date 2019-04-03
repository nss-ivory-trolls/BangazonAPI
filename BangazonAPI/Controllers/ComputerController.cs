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
    public class ComputerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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

        //Get all computers
        [HttpGet]
        public async Task<IActionResult> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM Computer c";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computerList = new List<Computer>();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                        };
                        computerList.Add(computer);
                    }
                    reader.Close();
                    return Ok(computerList);
                }
            }
        }

        //Get computer by Id
        [HttpGet("{id}", Name = "GetComputerById")]
        public async Task<IActionResult> GetComputerById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM Computer c
                                        WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Computer computer = null;

                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                        };
                    }
                    reader.Close();
                    return Ok(computer);
                }
            }
        }

        //Post a computer
        [HttpPost]
        public async Task<IActionResult> PostComputer([FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer)
                                        OUTPUT INSERTED.Id
                                        VALUES (@purchaseDate, @decomissionDate, @make, @manufacturer)";
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));

                    int newId = (int)cmd.ExecuteScalar();
                    computer.Id = newId;
                    return CreatedAtRoute("GetComputerById", new { id = newId }, computer);
                }
            }
        }

        //Edit a computer
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComputer([FromRoute] int id, [FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Computer
                                        SET PurchaseDate = @purchaseDate,
                                            DecomissionDate = @decomissionDate,
                                            Make = @make,
                                            Manufacturer = @manufacturer
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
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

        //Delete a computer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComputer([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Computer
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