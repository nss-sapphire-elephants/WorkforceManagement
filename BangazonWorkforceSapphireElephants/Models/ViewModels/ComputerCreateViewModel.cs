/*
 * CREATED BY HM
 */


using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime Purchased = new DateTime(2000,1,1);

        [Required]
        public DateTime? Assigned { get; set; }

        [Required]
        public int EmployeeId { get; set; }

       
            public ComputerCreateViewModel()
            {
                Employees = new List<Employee>();
            }

            public ComputerCreateViewModel(string connectionString)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, FirstName, LastName from Employee;";
                        SqlDataReader reader = cmd.ExecuteReader();

                        Employees = new List<Employee>();

                        while (reader.Read())
                        {
                            Employees.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            });
                        }
                        reader.Close();
                    }
                }
            }

        public List<Employee> Employees { get; set; }


            public List<SelectListItem> EmployeeOptions
            {
                get
                {
                    return Employees.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.FirstName
                    }).ToList();
                }
            }

    }
}
