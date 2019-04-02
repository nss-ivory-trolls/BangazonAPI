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
    public class DepartmentCRU
    {
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/department");
                string responseBody = await response.Content.ReadAsStringAsync();
                var CustomerList = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(CustomerList.Count > 0);
            }

        }

        [Fact]
        public async Task Test_Get_A_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/department/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, department.Id);

            }

        }

        [Fact]
        public async Task Test_Insert_A_Department()
        {
            using (var client = new APIClientProvider().Client)
            {

                Department HR = new Department
                {
                    Name= "HR",
                    Budget = 10000
                };

                var HRAsJSON = JsonConvert.SerializeObject(HR);
                var response = await client.PostAsync(
                    "/api/department",
                    new StringContent(HRAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newDepartment = JsonConvert.DeserializeObject<Department>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("HR", newDepartment.Name);

            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newDeptName = "Humam Resources";

            using (var client = new APIClientProvider().Client)
            {
                Department modifiedDept = new Department
                {
                    Id = 2,
                    Name = newDeptName,
                    Budget = 1000000
                };
                var modifiedDeptAsJSON = JsonConvert.SerializeObject(modifiedDept);

                var response = await client.PutAsync(
                    "/api/department/2",
                    new StringContent(modifiedDeptAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getDept = await client.GetAsync("/api/department/2");
                getDept.EnsureSuccessStatusCode();

                string getDeptBody = await getDept.Content.ReadAsStringAsync();

                Department newDept = JsonConvert.DeserializeObject<Department>(getDeptBody);

                Assert.Equal(HttpStatusCode.OK, getDept.StatusCode);
                Assert.Equal(newDeptName, newDept.Name);
            }
        }
    
    }
}
