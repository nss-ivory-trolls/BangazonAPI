
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
    public class TrainingProgramTest
    {
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    GET section
                */
                var response = await client.GetAsync("/api/TrainingProgram");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orderList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Delete_TrainingProgram()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/trainingprogram/1");

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
               
            }
        }

        [Fact]
        public async Task Test_Create_Training()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    CREATE section
                */
                var getOldList = await client.GetAsync("/api/trainingprogram");
                getOldList.EnsureSuccessStatusCode();

                string getOldListBody = await getOldList.Content.ReadAsStringAsync();
                var oldList = JsonConvert.DeserializeObject<List<TrainingProgram>>(getOldListBody);



                TrainingProgram trainingprogram = new TrainingProgram
                {
                    Name = "Learn to Code",
                    StartDate = new DateTime(2019,05,26),
                    EndDate = new DateTime(2019, 05, 28),
                    MaxAttendees = 15
                };

                var modifiedOrderAsJSON = JsonConvert.SerializeObject(trainingprogram);

                var response = await client.PostAsync(
                    "/api/trainingprogram",
                    new StringContent(modifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                /*
                    GET section
                    Verify that the Post operation was successful
                */
                var getOrder = await client.GetAsync("/api/trainingprogram");
                //getOrder.EnsureSuccessStatusCode();

                //string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                //var newList = JsonConvert.DeserializeObject<List<TrainingProgram>>(getOrderBody);
                var newProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);


                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(15, newProgram.MaxAttendees);
            }
        }

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            // New last name to change to and test

            using (var client = new APIClientProvider().Client)
            {

                int newmax = 16;
                TrainingProgram trainingprogram = new TrainingProgram
                {
                    Name = "How To Sell Cars",
                    StartDate = new DateTime(2020, 02, 14),
                    EndDate = new DateTime(2019, 02, 15),
                    MaxAttendees = newmax
                };

                var modifiedDeptAsJSON = JsonConvert.SerializeObject(trainingprogram);

                var response = await client.PutAsync(
                    "/api/TrainingProgram/4",
                    new StringContent(modifiedDeptAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();


                var getDept = await client.GetAsync("/api/TrainingProgram");
                getDept.EnsureSuccessStatusCode();

                string getDeptBody = await getDept.Content.ReadAsStringAsync();

            
                List<TrainingProgram> newList = JsonConvert.DeserializeObject<List<TrainingProgram>>(getDeptBody);

                Assert.Equal(HttpStatusCode.OK, getDept.StatusCode);
                Assert.Equal(newmax, newList[3].MaxAttendees);
            }
        }
    }
}
