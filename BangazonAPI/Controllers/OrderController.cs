//Ashwin

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

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute] int id, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "customer")
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from[order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id
                                            WHERE o.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();

                        if (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("oId"));
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {
                                Order order = new Order
                                {

                                    Id = orderId,
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
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
                            if (reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
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

                        };

                        reader.Close();
                        return Ok(orders.Values);

                    }

                    else if (_include == "products")
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from[order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id
                                                WHERE o.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();

                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("oId"));

                            if (!orders.ContainsKey(orderId))
                            {

                                if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    Order order = new Order
                                    {
                                        Id = orderId,
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                        ProductList = new List<Product>(),

                                    };
                                    orders.Add(orderId, order);
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                {
                                    Order order = new Order
                                    {
                                        Id = orderId,
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        ProductList = new List<Product>(),

                                    };
                                    orders.Add(orderId, order);
                                }
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

                    else
                    {
                        cmd.CommandText = $@"select o.Id as oId, op.Id as opId, p.Id as pId, c.Id as cId, o.CustomerId, o.PaymentTypeId, 
                                            op.OrderId, op.productId, pt.[Name], pt.AcctNumber, p.Title, p.Price,
                                            p.[Description], c.FirstName, c.LastName
                                            from[order] o
                                            left join paymentType pt on o.PaymentTypeId = pt.Id
                                            left join OrderProduct op on o.Id = op.OrderId
                                            left join product p on op.ProductId = p.Id
                                            left join customer c on o.CustomerId = c.Id WHERE o.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        Dictionary<int, Order> orders = new Dictionary<int, Order>();

                        if (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("oId"));
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
                        reader.Close();
                        return Ok(orders.Values);
                    }


                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@CustomerId, @PaymentTypeId)";
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));

                    int newId = (int)cmd.ExecuteScalar();
                    order.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, order);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE [Order]
                                            SET 
                                                CustomerId = @customerId,
                                                PaymentTypeId = @paymentTypeId
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
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
                if (!OrderExists(id))
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
                        cmd.CommandText = @"DELETE FROM [Order] WHERE Id = @id";
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
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, CustomerId, PaymentTypeId
                        FROM [Order]
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
