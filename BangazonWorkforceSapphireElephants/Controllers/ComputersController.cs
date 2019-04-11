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

                        if (!reader.IsDBNull(reader.GetOrdinal("CDecom")))
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

                        else if (reader.IsDBNull(reader.GetOrdinal("CDecom")))
                        {
                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                                Make = reader.GetString(reader.GetOrdinal("cMake")),
                                DecomissionDate = null,
                                Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                            };
                            computerList.Add(computer);
                        }
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
                        if (!reader.IsDBNull(reader.GetOrdinal("CDecom")))
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

                        else if (reader.IsDBNull(reader.GetOrdinal("CDecom")))
                        {
                             computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                                Make = reader.GetString(reader.GetOrdinal("cMake")),
                                DecomissionDate = null,
                                Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                            };
                           
                        }

                    }

                    reader.Close();
                    return View(computer);
                }
            }
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            ComputerCreateViewModel viewModel =
                new ComputerCreateViewModel();
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel viewModel)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO computer ([manufacturer], make, purchaseDate)
                                             VALUES (@manufacturer, @make, @purchaseDate)";
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", viewModel.Manufacturer));
                    cmd.Parameters.Add(new SqlParameter("@make", viewModel.Make));
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", viewModel.Purchased));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
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
            Computer computer = GetComputerById(id);
            if (computer == null)
            {
                return NotFound();
            }

            using (SqlConnection conn = Connection)
            {
                int? assignedcomputer = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS ComputerId,
                                        c.PurchaseDate, c. DecomissionDate,
                                        c.Make, c.Manufacturer, ce.ComputerId as ComputerEmployeeCID
                                        FROM Computer c LEFT JOIN ComputerEmployee ce on c.id = ce.ComputerId
                                        WHERE c.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())

                        assignedcomputer = reader.IsDBNull(reader.GetOrdinal("ComputerEmployeeCID")) ? (int?)null : (int?)reader.GetInt32(reader.GetOrdinal("ComputerEmployeeCID"));

                    if (assignedcomputer != null)
                    {
                        ComputerDeleteViewModel viewModel = new ComputerDeleteViewModel
                        {
                            Id = id,
                            Make = computer.Make,
                            Manufacturer = computer.Manufacturer,
                            PurchaseDate = computer.PurchaseDate,
                            DisplayDelete = false
                        };

                        reader.Close();
                        return View(viewModel);

                    }
                    else
                    {
                        ComputerDeleteViewModel viewModel = new ComputerDeleteViewModel
                        {
                            Id = id,
                            Make = computer.Make,
                            Manufacturer = computer.Manufacturer,
                            PurchaseDate = computer.PurchaseDate,
                            DisplayDelete = true
                        };
                        reader.Close();
                        return View(viewModel);
                    }
                }
            }
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM computer WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /*
       Fuction to get a cohort by ID
       */
        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select Id as cId, PurchaseDate as CPurchase, DecomissionDate as CDecom, Make as cMake, Manufacturer as CMan" +
                        " from computer" +
                        " WHERE computer.id =  @id"; ;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("CDecom")))
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

                        else if (reader.IsDBNull(reader.GetOrdinal("CDecom")))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                                Make = reader.GetString(reader.GetOrdinal("cMake")),
                                DecomissionDate = null,
                                Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                            };

                        }
                    }

                    reader.Close();

                    return computer;
                }
            }
        }

        /*
           Function to get all Computers
       */
        private List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select Id as cId, PurchaseDate as CPurchase, DecomissionDate as CDecom, Make as cMake, Manufacturer as CMan" +
                        " from computer";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("cId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("CPurchase")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("CDecom")),
                            Make = reader.GetString(reader.GetOrdinal("cMake")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("cMan"))
                        });
                    }
                    reader.Close();

                    return computers;
                }
            }

        }
    }
}