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
    public class CustomerController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public CustomerController(IConfiguration configuration)
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

        // GET: api/Customers
        [HttpGet]
        public IEnumerable<Customer> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    if (_include == "products")

                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName as FirstName,
                                            c.LastName as LastName,
                                            p.id as ProductId,
                                            p.producttypeid as ProductTypeId,
                                            p.title as Title,
                                            p.price as Price,
                                            p.Description as Description,
                                            p.Quantity as Quantity
                                            from Customer c
                                            left join Product as p on c.id =                            p.CustomerId
                                            WHERE 1 = 1 ";
                    }
                    else if (_include == "payments")
                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName as FirstName,
                                            c.LastName as LastName,
                                            pt.Id as PaymentTypeId,
                                            pt.Name as PaymentName,
                                            pt.AcctNumber as AccountNumber
                                            from Customer c
                                            left join PaymentType as pt on                            c.Id = pt.CustomerId
                                            WHERE 1 = 1";
                    }
                    else
                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName,
                                            c.LastName
                                            from Customer c
                                            WHERE 1 = 1";
                    }


                    if (!string.IsNullOrWhiteSpace(q))
                    {

                        cmd.CommandText += @" AND 
                                                (c.FirstName LIKE @q OR
                                                 c.LastName LIKE @q)";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
                    while (reader.Read())
                    {
                        int CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));

                        if (!customers.ContainsKey(CustomerId))

                        {
                            Customer newCustomer = new Customer
                            {
                                Id = CustomerId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                            customers.Add(CustomerId, newCustomer);
                        }
                        if (_include == "products")
                        {

                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))

                            {
                                Customer currentCustomer = customers[CustomerId];
                                currentCustomer.ProductList.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                    }
                                    );
                            }
                        }

                        if (_include == "payments")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {
                                Customer currentCustomer = customers[CustomerId];
                                currentCustomer.PaymentTypeList.Add(
                                    new PaymentType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                        Name = reader.GetString(reader.GetOrdinal("PaymentName")),
                                        AcctNumber = reader.GetInt32(reader.GetOrdinal("AccountNumber")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                    }
                                    );
                            }

                        }

                    }
                    reader.Close();
                    return customers.Values.ToList();
                }

            }
        }


        // GET: api/Customer/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id, string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "products")
                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName as FirstName,
                                            c.LastName as LastName,
                                            p.id as ProductId,
                                            p.producttypeid as ProductTypeId,
                                            p.title as Title,
                                            p.price as Price,
                                            p.Description as Description,
                                            p.Quantity as Quantity
                                            from Customer c
                                            left join Product as p on c.id =                            p.CustomerId
                                           WHERE c.id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                    }
                    else if (_include == "payments")
                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName as FirstName,
                                            c.LastName as LastName,
                                            pt.Id as PaymentTypeId,
                                            pt.Name as PaymentName,
                                            pt.AcctNumber as AccountNumber
                                            from Customer c
                                            left join PaymentType as pt on                            c.Id = pt.CustomerId
                                            WHERE c.id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                    }
                    else
                    {
                        cmd.CommandText = @"select c.id as CustomerId,
                                            c.FirstName,
                                            c.LastName
                                            from Customer c
                                            WHERE c.id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                    }

                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.CommandText += @" AND 
                                                (c.FirstName LIKE @q OR
                                                 c.LastName LIKE @q)";
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;
                    while (reader.Read())
                    {



                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        };


                        if (_include == "products")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                if (!customer.ProductList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("ProductId"))))
                                {
                                    customer.ProductList.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        Price = reader.GetInt32(reader.GetOrdinal("Price")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                    }
                                );

                                }                              

  
                            }
                        }

                        if (_include == "payments")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {

                                if (!customer.PaymentTypeList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))))
                                {
                                    customer.PaymentTypeList.Add(
                                    new PaymentType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                        Name = reader.GetString(reader.GetOrdinal("PaymentName")),
                                        AcctNumber = reader.GetInt32(reader.GetOrdinal("AccountNumber")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                    }
                                    );

                                }
                            }
                        }
                    }

                    reader.Close();                    
                    return Ok(customer);
                }                   
            }
        }
        

 


        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer newCustomer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO Customer (FirstName, LastName)
                                         OUTPUT INSERTED.Id
                                         Values(@FirstName, @LastName)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", newCustomer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", newCustomer.LastName));

                    int newId = (int)cmd.ExecuteScalar();
                    newCustomer.Id = newId;
                    return CreatedAtRoute("GetCustomer", new { id = newId }, newCustomer);

                }
            }
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Customer 
                                        SET FirstName = @FirstName, LastName = @LastName
                                        WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return NoContent();
                }
            }

        }        

        }

    
}
