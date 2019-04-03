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
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/TrainingProgram
        [HttpGet]
        public async Task<IActionResult> Get(string completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                        cmd.CommandText = $@"Select tp.id as tpId, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees,
                                            et.TrainingProgramId, et.EmployeeId AS employeeid, e.FirstName, e.LastName, e.departmentid, d.Name AS deptName
                                            From TrainingProgram tp
                                            right JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                            left JOIN Employee e ON e.id = et.EmployeeId 
                                            join Department d ON d.id = e.DepartmentId
                                            where 1=1 ";
                        if (completed == "false")
                        {
                            cmd.CommandText += @" AND EndDate >= GetDate()";
                        }

                        SqlDataReader reader = cmd.ExecuteReader();
                        Dictionary<int,TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();

                        while (reader.Read())
                        {
                            int tpId = reader.GetInt32(reader.GetOrdinal("tpId"));
                            if (!trainingPrograms.ContainsKey(tpId))
                            {

                                TrainingProgram trainingProgram = new TrainingProgram
                                    {
                                        Id = tpId,
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                        MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                        EmployeeList = new List<Employee>(),
                                    };

                                trainingPrograms.Add(tpId,trainingProgram);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("employeeid")))
                            {
                                TrainingProgram currentTrainingProgram = trainingPrograms[tpId];
                                currentTrainingProgram.EmployeeList.Add(
                                new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("employeeid")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentid")),
                                    Department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("departmentid")),
                                        Name = reader.GetString(reader.GetOrdinal("deptName"))

                                    }
                                }
                             );
                            }
                        }

                    reader.Close();

                    return Ok(trainingPrograms.Values);
                }

            }
        }

        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"Select tp.id as tpId, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees,
                                            et.TrainingProgramId, et.EmployeeId AS employeeid, e.FirstName, e.LastName, e.departmentid, d.Name AS deptName
                                            From TrainingProgram tp
                                            right JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                            left JOIN Employee e ON e.id = et.EmployeeId 
                                            join Department d ON d.id = e.DepartmentId
                                            where tp.id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();

                    while (reader.Read())
                    {
                        int tpId = reader.GetInt32(reader.GetOrdinal("tpId"));
                        if (!trainingPrograms.ContainsKey(tpId))
                        {

                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                                Id = tpId,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                EmployeeList = new List<Employee>(),
                            };

                            trainingPrograms.Add(tpId, trainingProgram);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("employeeid")))
                        {
                            TrainingProgram currentTrainingProgram = trainingPrograms[tpId];
                            currentTrainingProgram.EmployeeList.Add(
                            new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("employeeid")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentid")),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("departmentid")),
                                    Name = reader.GetString(reader.GetOrdinal("deptName"))

                                }
                            }
                         );
                        }
                    }

                    reader.Close();

                    return Ok(trainingPrograms.Values);
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @startDate, @endDate, @maxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

                    int newId = (int)cmd.ExecuteScalar();
                    trainingProgram.Id = newId;
                    return CreatedAtRoute("GetTrainingProgram", new { id = newId }, trainingProgram);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //Name, StartDate, EndDate, MaxAttendees
                        cmd.CommandText = @"UPDATE TrainingProgram
                                            SET 
                                                Name = @name,
                                                StartDate = @startDate,
                                                EndDate = @endDate,
                                                MaxAttendees = @maxAttendees
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
