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
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
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

        //Get all products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.Id, p.ProductTypeId, p.Title, p.Price, p.Description, p.Quantity, p.CustomerId,
                                        pt.Name AS ProductTypeName, c.FirstName, c.LastName
                                        FROM Product p
                                        LEFT JOIN ProductType pt ON p.ProductTypeId = pt.Id
                                        LEFT JOIN Customer c ON p.CustomerId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Product> productList = new List<Product>();

                    while (reader.Read())
                    {
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Name = reader.GetString(reader.GetOrdinal("ProductTypeName")),
                        };
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            ProductType = productType,
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Customer = customer
                        };
                        productList.Add(product);
                    }
                    reader.Close();
                    return Ok(productList);
                }
            }
        }

        //Get product by Id
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.Id, p.ProductTypeId, p.Title, p.Price, p.Description, p.Quantity, p.CustomerId,
                                        pt.Name AS ProductTypeName, c.FirstName, c.LastName
                                        FROM Product p
                                        LEFT JOIN ProductType pt ON p.ProductTypeId = pt.Id
                                        LEFT JOIN Customer c ON p.CustomerId = c.Id
                                        WHERE p.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Product product = null;

                    while (reader.Read())
                    {
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Name = reader.GetString(reader.GetOrdinal("ProductTypeName")),
                        };
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };
                        product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            ProductType = productType,
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Customer = customer
                        };
                    }
                    reader.Close();
                    return Ok(product);
                }
            }
        }

        //Post a product type
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Product (ProductTypeId, Title, Price, Description, Quantity, CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@productTypeId, @title, @price, @description, @quantity, @customerId)";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));

                    int newId = (int)cmd.ExecuteScalar();
                    product.Id = newId;
                    return CreatedAtRoute("GetProductById", new { id = newId }, product);
                }
            }
        }

        //Edit a product type
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Product
                                        SET ProductTypeId = @productTypeId,
                                            Title = @title,
                                            Price = @price,
                                            Description = @description,
                                            Quantity = @quantity,
                                            CustomerId = @customerId
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
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

        //Delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Product
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