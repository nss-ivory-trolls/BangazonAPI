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
    public class ProductTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
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

        //Get all product types
        [HttpGet]
        public async Task<IActionResult> GetAllProductTypes()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT pt.Id, pt.Name 
                                        FROM ProductType pt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<ProductType> productTypeList = new List<ProductType>();

                    while (reader.Read())
                    {
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        productTypeList.Add(productType);
                    }
                    reader.Close();
                    return Ok(productTypeList);
                }
            }
        }

        //Get product type by Id
        [HttpGet("{id}", Name = "GetProductTypeById")]
        public async Task<IActionResult> GetProductTypeById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT pt.Id, pt.Name 
                                        FROM ProductType pt
                                        WHERE pt.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    ProductType productType = null;

                    while (reader.Read())
                    {
                        productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();
                    return Ok(productType);
                }
            }
        }

        //Post a product type
        [HttpPost]
        public async Task<IActionResult> PostProductType([FromBody] ProductType productType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ProductType (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", productType.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    productType.Id = newId;
                    return CreatedAtRoute("GetProductTypeById", new { id = newId }, productType);
                }
            }
        }

        //Edit a product type
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType([FromRoute] int id, [FromBody] ProductType productType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE ProductType
                                        SET Name = @name
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@name", productType.Name));
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

        //Delete a product type
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ProductType
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