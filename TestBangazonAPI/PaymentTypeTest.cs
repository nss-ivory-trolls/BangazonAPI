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
    public class PaymentTypeTest
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    GET section
                */
                var response = await client.GetAsync("/api/PaymentType");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentList = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Delete_PaymentTypes()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/PaymentType/4");

                /*
                    DELETE section
                */
                var response1 = await client.GetAsync("/api/PaymentType/4");


                string responseBody = await response1.Content.ReadAsStringAsync();

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, response1.StatusCode);

            }
        }


        [Fact]
        public async Task Test_Create_Payment()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    CREATE section
                */
                var getOldList = await client.GetAsync("/api/PaymentType");
                getOldList.EnsureSuccessStatusCode();

                string getOldListBody = await getOldList.Content.ReadAsStringAsync();
                var oldList = JsonConvert.DeserializeObject<List<PaymentType>>(getOldListBody);



                PaymentType paymentType = new PaymentType
                {
                    Name = "VISA",
                    AcctNumber = 213213,
                    CustomerId = 2
                };

                var modifiedPaymentAsJSON = JsonConvert.SerializeObject(paymentType);

                var response = await client.PostAsync(
                    "/api/PaymentType",
                    new StringContent(modifiedPaymentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                /*
                    GET section
                    Verify that the Post operation was successful
                */
                var getPayments = await client.GetAsync("/api/PaymentType");
                getPayments.EnsureSuccessStatusCode();

                string getPaymentBody = await getPayments.Content.ReadAsStringAsync();
                var newList = JsonConvert.DeserializeObject<List<PaymentType>>(getPaymentBody);

                Assert.Equal(HttpStatusCode.OK, getPayments.StatusCode);
                Assert.True(newList.Count > oldList.Count);
            }
        }

       

        [Fact]
        public async Task Test_Modify_Payment()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                string newName = "AMEX";

                PaymentType modifiedPayment = new PaymentType
                {
                    Name = newName,
                    AcctNumber = 1000,
                    CustomerId = 1
                };

                var modifiedPaymentAsJson = JsonConvert.SerializeObject(modifiedPayment);

                var response = await client.PutAsync(
                    "/api/PaymentType/1",
                    new StringContent(modifiedPaymentAsJson, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getPayment = await client.GetAsync("/api/PaymentType/1");
                getPayment.EnsureSuccessStatusCode();

                string getPaymentBody = await getPayment.Content.ReadAsStringAsync();
                PaymentType newPayment = JsonConvert.DeserializeObject<PaymentType>(getPaymentBody);

                Assert.Equal(HttpStatusCode.OK, getPayment.StatusCode);
                Assert.Equal(newName, newPayment.Name);
            }
        }
    }
}
