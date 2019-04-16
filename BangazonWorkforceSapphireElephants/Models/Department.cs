using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

//Create by MR
namespace BangazonWorkforceSapphireElephants.Models
{
    public class Department
    {
        public int Id { get; set; }
        [DisplayName("Department")]
        public string Name { get; set; }
        public int Budget { get; set; }
        public int DepartmentSize
        { get
            {
                return Employees.Count();
            }
        }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
