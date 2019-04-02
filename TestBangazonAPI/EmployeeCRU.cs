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
    public class EmployeeCRU
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/employee");
                string responseBody = await response.Content.ReadAsStringAsync();
                var EmployeeList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(EmployeeList.Count > 0);
            }

        }

        [Fact]
        public async Task Test_Get_A_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/employee/1");
                string responseBody = await response.Content.ReadAsStringAsync();
                var Employee = JsonConvert.DeserializeObject<Employee>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, Employee.Id);

            }

        }

        [Fact]
        public async Task Test_Insert_An_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {

                Employee Andy = new Employee
                {
                    FirstName = "Andy",
                    LastName = "Collins",
                    IsSupervisor = true,
                    DepartmentId = 2,
                    ComputerId = 1
                };

                var AndyAsJSON = JsonConvert.SerializeObject(Andy);
                var response = await client.PostAsync(
                    "/api/employee",
                    new StringContent(AndyAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Andy", newEmployee.FirstName);

            }
        }

        [Fact]
        public async Task Test_Modify_Employee()
        {
            // New last name to change to and test
            string NewEmployeeFirstName = "A-Rod";

            using (var client = new APIClientProvider().Client)
            {
                Employee modifiedDept = new Employee
                {
                    Id = 1002,
                    FirstName = "A-Rod",
                    LastName = "Jenkins",
                    DepartmentId = 2
                };
                var modifiedDeptAsJSON = JsonConvert.SerializeObject(modifiedDept);

                var response = await client.PutAsync(
                    "/api/employee/1002",
                    new StringContent(modifiedDeptAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getEmployee = await client.GetAsync("/api/employee/1002");
                getEmployee.EnsureSuccessStatusCode();

                string getEmployeeBody = await getEmployee.Content.ReadAsStringAsync();

                Employee newEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);

                Assert.Equal(HttpStatusCode.OK, getEmployee.StatusCode);
                Assert.Equal(NewEmployeeFirstName, newEmployee.FirstName);
            }
        }
    }
}
