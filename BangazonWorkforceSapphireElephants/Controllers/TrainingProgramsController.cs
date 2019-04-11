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
        //    ****************************************************************
        //      GET: LIST of Training Programs That have not yet taken place
        //    ****************************************************************
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id,
                                            t.Name,
                                            t.StartDate,
                                            t.EndDate,
                                            t.MaxAttendees
                                        FROM TrainingProgram t
                                        WHERE StartDate > GETDATE()";
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        trainingPrograms.Add(trainingProgram);
                    }

                    reader.Close();
                    return View(trainingPrograms);
                }
            }
        }

        //    ****************************************************************
        //       GET: One Training Program
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
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    while (reader.Read())
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



        //    ***************************************
        //          GET: TrainingPrograms/Create
        //    ***************************************

        public ActionResult Create()
        {
            {
                TrainingProgramCreateViewModel viewModel = new TrainingProgramCreateViewModel();
                return View(viewModel);
            }
        }

        //    ******************************************
        //          POST: TrainingPrograms/Create
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
        
       

        //    *******************************************
        //      DELETE - GET: TrainingPrograms/Delete/5
        //    *******************************************        

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
        //    ***********************************
        //      POST: TrainingProgram/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM TrainingProgram  WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();                        

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(trainingProgram);
            }
        }

        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id, t.Name, t.StartDate, t.EndDate, t.MaxAttendees 
                                        FROM TrainingProgram t
                                        WHERE  t.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
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
    }
}