using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class CustomerCRU
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customer");
                string responseBody = await response.Content.ReadAsStringAsync();
                var CustomerList = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(CustomerList.Count > 0);
            }

        }
        [Fact]
        public async Task Test_Get_A_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customer/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var Customer = JsonConvert.DeserializeObject<Customer>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, Customer.Id);
                
            }

        }

        [Fact]
        public async Task Test_Insert_A_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {

                Customer LeonBridges = new Customer
                {
                    FirstName = "Leon",
                    LastName = "Bridges"
                };

                var LeonBridgesAsJSON = JsonConvert.SerializeObject(LeonBridges);
                var response = await client.PostAsync(
                    "/api/customer",
                    new StringContent(LeonBridgesAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newLeonBridges = JsonConvert.DeserializeObject<Customer>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Leon", newLeonBridges.FirstName);

            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newCustomerName = "LEON";

            using (var client = new APIClientProvider().Client)
            {
                Customer modifiedCustomer = new Customer
                {
                    Id = 2, 
                    FirstName = newCustomerName,
                    LastName = "Bridges"
                };
                var modifiedCustomerAsJSON = JsonConvert.SerializeObject(modifiedCustomer);

                var response = await client.PutAsync(
                    "/api/customer/2",
                    new StringContent(modifiedCustomerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getCustomer = await client.GetAsync("/api/customer/2");
                getCustomer.EnsureSuccessStatusCode();

                string getCustomerBody = await getCustomer.Content.ReadAsStringAsync();

                Customer newCustomer = JsonConvert.DeserializeObject<Customer>(getCustomerBody);

                Assert.Equal(HttpStatusCode.OK, getCustomer.StatusCode);
                Assert.Equal(newCustomerName, newCustomer.FirstName);
            }
        }
    }
}