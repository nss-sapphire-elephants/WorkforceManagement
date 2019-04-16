using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforceSapphireElephants.Models;
using BangazonWorkforceSapphireElephants.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforceSapphireElephants.Controllers
{
    public class TrainingProgramsController : Controller    
    {
        private readonly IConfiguration _configuration;

        public TrainingProgramsController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        //      -- Created by CW
        //    ****************************************************************
        //                   GET: LIST of FUTURE Training Programs
        //    ****************************************************************

        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id,
                                    t.[Name],
                                    t.StartDate,
                                    t.EndDate,
                                    t.MaxAttendees
                                FROM TrainingProgram t
                                WHERE EndDate > GETDATE()";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> futurePrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram futureProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        futurePrograms.Add(futureProgram);
                    }

                    reader.Close();
                    return View(futurePrograms);
                }
            }
        }

        //      -- Created by CW
        //    ****************************************************************
        //                   GET: LIST of PAST Training Programs
        //    ****************************************************************

        public ActionResult Past()
        {
            DateTime today = DateTime.UtcNow.AddDays(01);
            string filterDate = $"{today.Year}-{today.Month}-{today.Day}";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id, 
                                            [Name], 
                                            StartDate, 
                                            EndDate, 
                                            MaxAttendees
                                        FROM TrainingProgram
                                        WHERE EndDate < '{filterDate}'
                                        ORDER BY StartDate";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> pastPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram pastProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        pastPrograms.Add(pastProgram);
                    }

                    reader.Close();
                    return View(pastPrograms);
                }
            }
        }

        //      -- Created by Anupama
        //    ****************************************************************
        //                   GET: Details---Training Programs
        //    ****************************************************************


        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
            SELECT tp.Id as TrainingProgramId,
                tp.[Name],
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id as EmployeeId,
                e.FirstName,
                e.LastName,
                e.IsSupervisor,
                d.Id as DepartmentId
                
            FROM TrainingProgram tp
            
            left join EmployeeTraining et on tp.Id = et.TrainingProgramId
            left join Employee e on e.Id = et.EmployeeId
                    left join Department d on e.DepartmentId = d.Id
                    WHERE tp.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader(
                        );

                    TrainingProgram trainingProgram = null;

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                EmployeeList = new List<Employee>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {


                            trainingProgram.EmployeeList.Add(
                                 new Employee
                                 {
                                     Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                     FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                     LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                     IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                     DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))

                                 });

                        }


                    }

                    reader.Close();

                    return View(trainingProgram);

                }
            }
        }


        //      -- Created by CW
        //    *******************************************************
        //      GET: TrainingProgram View Model to Create a new One
        //    *******************************************************

        public ActionResult Create()
        {
            {
                TrainingProgramCreateViewModel viewModel = new TrainingProgramCreateViewModel();
                return View(viewModel);
            }
        }
        //      -- Created by CW
        //    ******************************************
        //          POST: The TrainingProgram Created
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgramCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram
                                    (Name, StartDate, EndDate, MaxAttendees)
                                    VALUES (@name, @startDate, @endDate, @maxAttendees)";
                        cmd.Parameters.Add(new SqlParameter("@name", viewModel.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", viewModel.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", viewModel.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", viewModel.MaxAttendees));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(viewModel);
            }
        }

        //      -- Created by Anupama
        //    ****************************************************************
        //                   GET: TrainingPrograms/Edit
        //    ****************************************************************

        
        public ActionResult Edit(int id)
        {
            {
                TrainingProgram trainingProgrm = GetTrainingProgramByIdforEdit(id);
                if (trainingProgrm == null)
                {
                    return NotFound();
                }

                 TrainingProgramEditViewModel viewModel = new TrainingProgramEditViewModel
                {

                TrainingProgram = trainingProgrm
                 };

                 return View(viewModel);
               
            }
        }

        //      -- Created by Anupama
        //    ****************************************************************
        //                   POST: TrainingPrograms/Edit
        //    ****************************************************************
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Edit(int id, TrainingProgramEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram 
                                           SET Name = @name, 
                                               StartDate = @startDate,
                                               EndDate = @endDate, 
                                               MaxAttendees = @maxAttendees
                                         WHERE Id = @Id;";
                       cmd.Parameters.Add(new SqlParameter("@name", viewModel.TrainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", viewModel.TrainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", viewModel.TrainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", viewModel.TrainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@Id", id));

                        

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                
                return View(viewModel);
            }

        }
        //      -- Created by Anupama
        //    ****************************************************************
        //                   Method: GetTrainingProgramByIdforEdit
        //    ****************************************************************


        private TrainingProgram GetTrainingProgramByIdforEdit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id as TrainingProgramId,
                [Name],
                StartDate,
                EndDate,
                MaxAttendees
               

            FROM TrainingProgram 

            
                    WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {


                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),

                        };




                    }

                    reader.Close();

                    return trainingProgram;

                }

            }
        }


        //      -- Created by CW
        //    ****************************************************
        //      DELETE - GET: TrainingProgram to be Deleted by Id
        //    ****************************************************        

        public ActionResult Delete(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);
            if (trainingProgram == null)
            {
                return NotFound();
            }
            else
            {
                return View(trainingProgram);
            }
        }
        //      -- Created by CW
        //    ***********************************
        //      POST: TrainingProgram/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM EmployeeTraining WHERE TrainingProgramId = @id
                        DELETE FROM TrainingProgram  WHERE Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();                        

                    return RedirectToAction(nameof(Index));
                }
            }            
        }

        //      -- Created by CW
        //    *************************************************
        //         Method - GetTrainingProgramById(int id)
        //    *************************************************  
        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id AS TDID, t.Name, t.StartDate, t.EndDate, t.MaxAttendees 
                                        FROM TrainingProgram t
                                        WHERE  t.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TDID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                    }

                    reader.Close();
                    return trainingProgram;
                }
            }
        }

        //      -- Created by CW
        //    *************************************************
        //         Method - GetTrainingProgramPastDate()
        //    *************************************************  

    }
}