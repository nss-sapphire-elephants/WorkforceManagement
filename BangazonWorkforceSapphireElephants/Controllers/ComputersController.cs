using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforceSapphireElephants.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforceSapphireElephants.Controllers
{
    public class ComputersController : Controller
    {

        /*
         Establishing connection
        */
        private readonly IConfiguration _configuration;

        public ComputersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
        /*
        Establishing connection
       */


        // GET: Computers
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select Id as cId, PurchaseDate as CPurchase, DecomissionDate as CDecom, Make as cMake, Manufacturer as CMan" +
                        " from computer";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computerList = new List<Computer>();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("cId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("CDecom")),
                            Make = reader.GetString(reader.GetOrdinal("cMake")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                        };
                        computerList.Add(computer);
                    }

                    reader.Close();
                    return View(computerList);
                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "select Id as cId, PurchaseDate as CPurchase, DecomissionDate as CDecom, Make as cMake, Manufacturer as CMan" +
                        " from computer" +
                        " WHERE computer.id =  @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();



                    Computer computer = null;
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("cId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("CDecom")),
                            Make = reader.GetString(reader.GetOrdinal("cMake")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                        };

                    }

                    reader.Close();
                    return View(computer);
                }
            }
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}