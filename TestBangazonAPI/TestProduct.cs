using BangazonAPI.Models;
using TestBangazonAPI;
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
    public class TestProduct
    {
        [Fact]
        public async Task Test_Get_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/product");

                string responseBody = await response.Content.ReadAsStringAsync();

                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Product_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/product/1");

                string responseBody = await response.Content.ReadAsStringAsync();

                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(product.Title);
            }
        }

        [Fact]
        public async Task Test_Post_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product product = new Product
                {
                    ProductTypeId = 1,
                    Title = "iPhone",
                    Price = 1000,
                    Description = "It's an iPhone",
                    Quantity = 100,
                    CustomerId = 1
                };

                var productJson = JsonConvert.SerializeObject(product);

                var response = await client.PostAsync(
                    "/api/product",
                    new StringContent(productJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newProduct = JsonConvert.DeserializeObject<Product>(responseBody);


                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(product.ProductTypeId, newProduct.ProductTypeId);
                Assert.Equal(product.Title, newProduct.Title);
                Assert.Equal(product.Price, newProduct.Price);
                Assert.Equal(product.Description, newProduct.Description);
                Assert.Equal(product.Quantity, newProduct.Quantity);
                Assert.Equal(product.CustomerId, newProduct.CustomerId);
            }
        }

        [Fact]
        public async Task Test_Put_Product()
        {
            string title = "iPhone X";
            using (var client = new APIClientProvider().Client)
            {
                Product modifiedProduct = new Product
                {
                    ProductTypeId = 1,
                    Title = title,
                    Price = 1000,
                    Description = "It's an iPhone",
                    Quantity = 100,
                    CustomerId = 1
                };
                var productJson = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "/api/product/4",
                    new StringContent(productJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getProduct = await client.GetAsync("/api/product/4");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(title, newProduct.Title);
            }
        }

        [Fact]
        public async Task Test_Delete_Product()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/product/4");

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}
