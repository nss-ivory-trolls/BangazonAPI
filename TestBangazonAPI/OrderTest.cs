//Ashwin
using System;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System.Net;
using BangazonAPI.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class OrderTest
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    GET section
                */
                var response = await client.GetAsync("/api/order");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orderList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Delete_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/Order/1");

                /*
                    DELETE section
                */
                var getOldList = await client.GetAsync("/api/Order");
                getOldList.EnsureSuccessStatusCode();

                string getOldListBody = await getOldList.Content.ReadAsStringAsync();
                var oldList = JsonConvert.DeserializeObject<List<Order>>(getOldListBody);

                var getOrder = await client.GetAsync("/api/Order");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                var newList = JsonConvert.DeserializeObject<List<Order>>(getOrderBody);
                /*
                    ASSERT
                */
                Assert.False(newList.Count > oldList.Count);

            }
        }

        [Fact]
        public async Task Test_Create_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    CREATE section
                */
                var getOldList = await client.GetAsync("/api/Order");
                getOldList.EnsureSuccessStatusCode();

                string getOldListBody = await getOldList.Content.ReadAsStringAsync();
                var oldList = JsonConvert.DeserializeObject<List<Order>>(getOldListBody);



                Order order = new Order
                {
                    CustomerId = 4,
                    PaymentTypeId = 4,
                };

                var modifiedOrderAsJSON = JsonConvert.SerializeObject(order);

                var response = await client.PostAsync(
                    "/api/Order",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                /*
                    GET section
                    Verify that the Post operation was successful
                */
                var getOrder = await client.GetAsync("/api/Order");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                var newList = JsonConvert.DeserializeObject<List<Order>>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.True(newList.Count > oldList.Count);
            }
        }

        [Fact]
        public async Task Test_Modify_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                int newId = 4;

                Order modifiedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = newId,
                };

                var modifiedOrderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    "/api/Order/1",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                //var getOrder = await client.GetAsync("/api/Order/1");
                //getOrder.EnsureSuccessStatusCode();

                //string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                //Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);
                var getOrder = await client.GetAsync("/api/Order");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                var newList = JsonConvert.DeserializeObject<List<Order>>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(newId, newList[0].PaymentTypeId);
            }
        }
    }
}
