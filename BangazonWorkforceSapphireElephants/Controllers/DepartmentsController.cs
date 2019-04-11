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

//CREATED BY MR
namespace BangazonWorkforceSapphireElephants.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _configuration;

        public DepartmentsController(IConfiguration configuration)
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
        // GET: Departments
        [HttpGet]
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"select d.id as departmentId, d.[name], d.budget, e.id as EmployeeId, e.firstname, e.lastname
                                            FROM Department d LEFT JOIN Employee e
                                            on d.id = e.departmentid";

                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Department> departments = new Dictionary<int, Department>();
                    while (reader.Read())
                    {
                        int departmentId = reader.GetInt32(reader.GetOrdinal("departmentId"));
                        if (!departments.ContainsKey(departmentId))

                        {
                            Department newDepartment = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };

                            departments.Add(departmentId, newDepartment);
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {

                            Department currentDepartment = departments[departmentId];
                            currentDepartment.Employees.Add(
                            new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))

                            });
                        }

                    }

                    reader.Close();

                    return View(departments.Values.ToList());
                }
            }
        }



        // GET: Departments/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT d.Id as departmentId, d.[name], d.Budget, e.Id as employeeId, e.FirstName as FirstName, 
                                        e.LastName as LastName
                                        FROM Department d
                                        left join Employee e
                                        ON e.DepartmentId = d.Id
                                        where d.id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department departments = null;

                    while (reader.Read())
                    {
                        if (departments == null)
                        {
                            departments = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            };
                        }


                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            departments.Employees.Add(
                                new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }
                            );
                        }

                    }

                    reader.Close();

                    return View(departments);
                }
            }
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            DepartmentCreateViewModel viewModel =
                new DepartmentCreateViewModel();
            return View(viewModel);
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentCreateViewModel viewModel)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO department (name, budget)
                                             OUTPUT INSERTED.Id
                                             VALUES (@name, @budget)";
                    cmd.Parameters.Add(new SqlParameter("@name", viewModel.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", viewModel.Budget));

                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        private Department getDepartmentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT d.Id as DepartmentId, d.[Name]  as dName, d.Budget as Budget " +
                        "From Department d " +
                        "Where d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Name = reader.GetString(reader.GetOrdinal("dName")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };
                    }

                    reader.Close();
                    return (department);
                }
            }
        }
        /*
                public ActionResult Edit(int id)
                {
                    Department department = getDepartmentById(id);
                    DepartmentCreateViewModel viewModel = new DepartmentCreateViewModel
                    {
                        Department = department
                    };
                    return View(viewModel);
                }
                [HttpPost]
                public ActionResult Edit(int id, DepartmentCreateViewModel viewModel)
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"UPDATE Department
                                                    SET [Name] = @name
                                                    WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@name", viewModel.Name));
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.ExecuteNonQuery();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }*/
    }
}
