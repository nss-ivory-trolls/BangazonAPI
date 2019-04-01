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
        public async Task<IActionResult> Get(string _include, string completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "products")
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId,  
                                            op.OrderId, op.productId, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from [order] o
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id";
                    }
                    if (completed == "false")
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from [order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id
                                            where o.paymentTypeId is null";
                    }
                    if (completed == "true")
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from [order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id
                                            where o.paymentTypeId is not null";
                    }
                    else
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from[order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id";
                    }

                        SqlDataReader reader = cmd.ExecuteReader();

                        Dictionary<int, Order> orders = new Dictionary<int, Order>();

                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("oId"));

                        if (_include == "customers")
                        {
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
                                    ProductList = new List<Product>(),
                                };

                                orders.Add(orderId, order);
                            }
                        }

                        else if (_include == "products")
                        {
                            if (!orders.ContainsKey(orderId))
                            {
                                Order order = new Order
                                {
                                    Id = orderId,
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
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
                        else if (completed == "false")
                        {
                            if (!orders.ContainsKey(orderId))
                            {
                                if (reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    Order order = new Order
                                    {
                                        Id = orderId,
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    };
                                    orders.Add(orderId, order);
                                }
                            }
                        }

                        else
                        {
                            if (!orders.ContainsKey(orderId))
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    Order order = new Order
                                    {
                                        Id = orderId,
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    };
                                    orders.Add(orderId, order);
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    Order order = new Order
                                    {
                                        Id = orderId,
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    };
                                    orders.Add(orderId, order);
                                }
                            }
                        }
                            


                        }
                        reader.Close();

                        return Ok(orders.Values);
 
                }

            }
        }


    }
}
