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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                   
                        cmd.CommandText = $@"select o.Id as oId, c.Id as cId, op.Id as opId, p.Id as pId, pt.Id as ptId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, 
                                            p.[Description], c.FirstName, c.LastName, p.Price
                                            from [order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Order> orders = new Dictionary<int, Order>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("oId"));

                        if (!orders.ContainsKey(orderId))
                        {
                            Order order = new Order
                            {
                                Id = orderId,
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                },
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                PaymentType = new PaymentType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ptId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber"))
                                },

                                ProductList = new List<Product>(),
                            };

                            orders.Add(orderId, order);
                        }

                       
                        if (!reader.IsDBNull(reader.GetOrdinal("pId")))
                        {
                                Order currentOrder = orders[orderId];
                                currentOrder.ProductList.Add(
                                new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("pId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price"))
                                }
                            );                   
                        }

                    }
                    reader.Close();

                    return Ok(orders.Values);
                }

            }
        }


    }
}
