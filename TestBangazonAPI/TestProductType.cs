using BangazonAPI.Models;
using BangazonAPI.Tests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BangazonAPI.Tests
{
    public class TestProductType
    {
        [Fact]
        public async Task Test_Get_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/producttype");

                string responseBody = await response.Content.ReadAsStringAsync();

                var productTypeList = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypeList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_ProductType_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/producttype/1");

                string responseBody = await response.Content.ReadAsStringAsync();

                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(productType.Name);
            }
        }

        [Fact]
        public async Task Test_Post_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType productType = new ProductType
                {
                    Name = "Clothing"
                };

                var productTypeJson = JsonConvert.SerializeObject(productType);

                var response = await client.PostAsync(
                    "/api/producttype",
                    new StringContent(productTypeJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);


                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(productType.Name, newProductType.Name);
            }
        }

        [Fact]
        public async Task Test_Put_ProductType()
        {
            string name = "Food";
            using (var client = new APIClientProvider().Client)
            {
                ProductType modifiedProductType = new ProductType
                {
                    Name = name
                };
                var productTypeJson = JsonConvert.SerializeObject(modifiedProductType);

                var response = await client.PutAsync(
                    "/api/producttype/5",
                    new StringContent(productTypeJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getProductType = await client.GetAsync("/api/producttype/5");
                getProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProductType.Content.ReadAsStringAsync();
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, getProductType.StatusCode);
                Assert.Equal(name, newProductType.Name);
            }
        }

        [Fact]
        public async Task Test_Delete_ProductType()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/producttype/5");

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}
