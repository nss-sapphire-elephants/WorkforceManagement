//Author: Nick Hansen
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
            Employee employee = GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            EmployeeEditViewModel viewModel = new EmployeeEditViewModel 
            //are these the things that need to be listed here? what are they = to?
            {
                Employee = employee,
                AddTrainingProgramList = GetAllAvailableTrainingPrograms(id),
                EnrolledTrainingProgramsList = GetAllEnrolledTrainingPrograms(id),
                ComputersList = GetAllAvailableComputers(),
                DepartmentsList = GetAllDepartments()
                
            };

            return View(viewModel);
        }
        
        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"Select e.Id as EmployeeId, e.FirstName, e.LastName, d.Name as DepartmentName,                   co.Make, tp.Name as TrainingProgram, tp.Id as TrainingProgramId, tp.StartDate,                  tp.EndDate, co.Id as ComputerId
                                            From Employee e
                                            Left Join Department d
                                                On e.DepartmentId = d.Id
                                            Left Join ComputerEmployee c
                                                On c.EmployeeId = e.Id
                                            Left Join Computer co
                                             On co.Id = c.ComputerId
                                            Left Join EmployeeTraining et
                                                On e.id = et.EmployeeId as TrainingEmployeeId
                                            Left Join TrainingProgram tp
                                                On et.TrainingProgramId = tp.Id
                                            Where e.Id = @id

                                            UPDATE Employee
                                            SET FirstName = @FirstName,
                                                DepartmentId = @DepartmentId,
                                                ComputerId = @ComputerId                                             
                                            WHERE EmployeeId = @id

                                            UPDATE EmployeeTraining tp
                                            SET TrainingEmployeeId = @EmployeeTrainingId
                                            WHERE TrainingEmployeeId = @id


                                            //delete training program list and re add instead of update

                                            ";

                        cmd.Parameters.Add(new SqlParameter("@LastName", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", viewModel.Employee.DepartmentId)); //need access to the department class
                        cmd.Parameters.Add(new SqlParameter("@ComputerId", viewModel.)); //need access to the computer class
                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", viewModel.EnrolledTrainingProgramsList));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

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
        private Employee GetEmployeeById(int id) //Get employee by ID with access to computers, training programs, departments
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select e.Id as EmployeeId, e.FirstName, e.LastName, d.Name as DepartmentName, co.Make, 
                                        tp.Name as TrainingProgram, tp.Id as TrainingProgramId, tp.StartDate, tp.EndDate, co.Id as ComputerId
                                            From Employee e
                                            Left Join Department d
                                                On e.DepartmentId = d.Id
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
                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>(); 

                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")); // employee id to pass into each if statement

                        if (!employees.ContainsKey(employeeId)) //if the employee id doesn't exist in the dictionary, instantiate one
                        {
                            employee = new Employee
                            {

                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                trainingPrograms = new List<TrainingProgram>(),
                                computer = new Computer(),
                                department = new Department
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                }
                            };
                            employees.Add(employeeId, employee); // add the employee object and id to the dictionary
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId"))) //if there is a computer id related to the current employee in the while loop
                        {
                            Employee currentemployee = employees[employeeId]; //access a speicific employee in the dictionary
                            currentemployee.computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                Make = reader.GetString(reader.GetOrdinal("Make"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            Employee currentemployee = employees[employeeId];

                            if (!currentemployee.trainingPrograms.Any(t => t.Id == reader.GetInt32(reader.GetOrdinal("TrainingProgramId"))))
                            {
                                currentemployee.trainingPrograms.Add(new TrainingProgram
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgram")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                });
                            }
                        }                     
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
        private List<Computer> GetAllAvailableComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Make
                                            FROM Computer c
                                            LEFT JOIN ComputerEmployee ce
                                            ON c.Id = ce.ComputerId
                                            WHERE ce.UnassignDate IS NOT NULL";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> availableComputers = new List<Computer>();

                    while (reader.Read())
                    {   
                        availableComputers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                                                                         
                        });
                    }
                    reader.Close();

                    return availableComputers;
                }
            }
        }
        private List<TrainingProgram> GetAllAvailableTrainingPrograms(int id)
        {    
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) 
                {
                    // Searches database for training programs where a specific user has not registered and the training program has not started yet
                    cmd.CommandText = @"select tp.Id, tp.Name
                                        FROM TrainingProgram tp
                                        LEFT JOIN EmployeeTraining et
                                        ON tp.Id = et.TrainingProgramId
                                        WHERE  et.EmployeeId != @id AND StartDate >= CURRENT_TIMESTAMP ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> availableTrainingPrograms = new List<TrainingProgram>();
                
                    while (reader.Read())
                    {

                        availableTrainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }
                    reader.Close();

                    return availableTrainingPrograms;
                }
            }
        }
        private List<TrainingProgram> GetAllEnrolledTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select tp.Id, tp.Name
                                        FROM TrainingProgram tp
                                        LEFT JOIN EmployeeTraining et
                                        ON tp.Id = et.TrainingProgramId
                                        WHERE  et.EmployeeId = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> enrolledTrainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        enrolledTrainingPrograms.Add(new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),

                        });
                    }
                    reader.Close();

                    return enrolledTrainingPrograms;
                }
            }
        }
    }
}
