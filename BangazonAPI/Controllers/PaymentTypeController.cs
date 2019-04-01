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
    public class PaymentTypeController : Controller
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
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

        [HttpGet]
        public async Task<IActionResult> Get(string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "customer")
                    {
                        cmd.CommandText = $@"select pt.Id as ptId, pt.AcctNumber, pt.[Name], pt.CustomerId, c.Id as cId, c.FirstName, c.LastName
                                            from PaymentType pt
                                            left join Customer c on pt.CustomerId = c.Id";
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<PaymentType> paymentTypes = new List<PaymentType>();

                        while (reader.Read())
                        {
                            PaymentType paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ptId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                }
                            };

                            paymentTypes.Add(paymentType);
                        }

                        reader.Close();

                        return Ok(paymentTypes);
                    }

                    else
                    {
                        cmd.CommandText = $@"select pt.Id as ptId, pt.AcctNumber, pt.[Name], pt.CustomerId
                                            from PaymentType pt";
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<PaymentType> paymentTypes = new List<PaymentType>();

                        while (reader.Read())
                        {
                            PaymentType paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ptId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            };

                            paymentTypes.Add(paymentType);
                        }

                        reader.Close();

                        return Ok(paymentTypes);
                    }
                }
            }
        }

        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id, string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "customer")
                    {
                        cmd.CommandText = $@"select pt.Id as ptId, pt.AcctNumber, pt.[Name], pt.CustomerId, c.Id as cId, c.FirstName, c.LastName
                                            from PaymentType pt
                                            left join Customer c on pt.CustomerId = c.Id";
                        SqlDataReader reader = cmd.ExecuteReader();
                        PaymentType paymentType = null;

                        if (reader.Read())
                        {
                            paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ptId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                }
                            };

                        }

                        reader.Close();

                        return Ok(paymentType);
                    }

                    else
                    {
                        cmd.CommandText = $@"select pt.Id as ptId, pt.AcctNumber, pt.[Name], pt.CustomerId
                                            from PaymentType pt";
                        SqlDataReader reader = cmd.ExecuteReader();
                        PaymentType paymentType = null;

                        if (reader.Read())
                        {
                             paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ptId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            };

                        }

                        reader.Close();

                        return Ok(paymentType);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PaymentType (Name, AcctNumber, CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @acctnumber, @customerid)";
                    cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@acctnumber", paymentType.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@customerid", paymentType.CustomerId));

                    int newId = (int)cmd.ExecuteScalar();
                    paymentType.Id = newId;
                    return CreatedAtRoute("GetPaymentType", new { id = newId }, paymentType);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE PaymentType
                                            SET Name = @name,
                                                AcctNumber = @AcctNumber,
                                                CustomerId = @CustomerId
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", paymentType.Name));
                        cmd.Parameters.Add(new SqlParameter("@AcctNumber", paymentType.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", paymentType.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM PaymentType WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, AcctNumber, CustomerId
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}