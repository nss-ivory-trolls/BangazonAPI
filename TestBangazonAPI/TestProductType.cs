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

        /*[Fact]
        public async Task Test_Post_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                Cohort cohort = new Cohort
                {
                    Name = "Cohort 100"
                };

                var cohortJson = JsonConvert.SerializeObject(cohort);

                var response = await client.PostAsync(
                    "/api/cohorts",
                    new StringContent(cohortJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newCohort = JsonConvert.DeserializeObject<Cohort>(responseBody);


                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(cohort.Name, newCohort.Name);
            }
        }

        [Fact]
        public async Task Test_Put_Cohort()
        {
            string name = "Cohort 101";
            using (var client = new APIClientProvider().Client)
            {
                Cohort modifiedCohort = new Cohort
                {
                    Name = name
                };
                var cohortJson = JsonConvert.SerializeObject(modifiedCohort);

                var response = await client.PutAsync(
                    "/api/cohorts/4",
                    new StringContent(cohortJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getCohort = await client.GetAsync("/api/cohorts/4");
                getCohort.EnsureSuccessStatusCode();

                string getCohortBody = await getCohort.Content.ReadAsStringAsync();
                Cohort newCohort = JsonConvert.DeserializeObject<Cohort>(getCohortBody);

                Assert.Equal(HttpStatusCode.OK, getCohort.StatusCode);
                Assert.Equal(name, newCohort.Name);
            }
        }

        [Fact]
        public async Task Test_Delete_Cohort()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/cohorts/4");


                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }*/
    }
}
