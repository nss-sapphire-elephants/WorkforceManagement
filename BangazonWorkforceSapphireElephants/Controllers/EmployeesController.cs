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
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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
        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.Name as DepartmentName, d.id as DepartmentId
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        ON e.DepartmentId = d.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            }
                        };

                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            {
                Employee employee = GetEmployeeById(id);
                if (employee == null)
                {
                    return NotFound();
                }

                else
                {
                    return View(employee);
                }
            }
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            EmployeeCreateViewModel viewModel = new EmployeeCreateViewModel(_config.GetConnectionString("DefaultConnection"));

            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId)
                                             VALUES (@FirstName, @LastName, @IsSuperVisor, @DepartmentId)";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", viewModel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@IsSuperVisor", viewModel.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", viewModel.Employee.DepartmentId));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                viewModel.Departments = GetAllDepartments();
                return View(viewModel);
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
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

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
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
        //-------------------------------------------   MODULAR GET EMPLOYEE BY ID  ------------------------------------
        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select e.Id as EmployeeId, e.FirstName, e.LastName, d.Name as DepartmentName,  co.Make, tp.Name as TrainingProgram
                                            From Employee e
                                            Left Join Department d
                                            On e.Id = d.Id
                                            Left Join ComputerEmployee c
                                            On c.EmployeeId = e.Id
                                            Left Join Computer co
                                            On co.Id = c.ComputerId
                                            Left Join EmployeeTraining et
                                            On e.id = et.EmployeeId
                                            Left Join TrainingProgram tp
                                            On et.TrainingProgramId = tp.Id
                                            Where e.Id = @id";
                                            
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            },
                            computer = new Computer
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make"))
                            },
                            trainingProgram = new TrainingProgram
                            {
                                Name = reader.GetString(reader.GetOrdinal("TrainingProgram"))
                            }
                         };
                    }

                    reader.Close();

                    return employee;
                }
            }

        }
        //-------------------------------------         MODULAR DEPARTMENT DROPDOWN            -----------------------------------------
        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name 
                                        FROM Department;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();

                    return departments;
                }
            }
        }
    }
}