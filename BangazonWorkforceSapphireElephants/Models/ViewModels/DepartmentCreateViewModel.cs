using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Created by MR

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class DepartmentCreateViewModel
    {
        public Department Department { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public int DepartmentSize
        {
            get
            {
                return Employees.Count();
            }
        }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
