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
        //                   GET: LIST of Training Programs
        //    ****************************************************************

        public ActionResult Index(string option)
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
                                WHERE EndDate > GETDATE()";                       
                    
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
    }
}