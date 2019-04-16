using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public EmployeeEditViewModel()
        {
            Employee = new Employee();
            DepartmentsList = new List<Department>();
            ComputersList = new List<Computer>();
            AddTrainingProgramList = new List<TrainingProgram>();
            EnrolledTrainingProgramsList = new List<TrainingProgram>();
        }

        public Employee Employee { get; set; }
        public List<Department> DepartmentsList { get; set; }
        public List<Computer> ComputersList { get; set; }
        public List<TrainingProgram> AddTrainingProgramList { get; set; }
        public List<TrainingProgram> EnrolledTrainingProgramsList { get; set; }
      
        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                return DepartmentsList.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
        public List<SelectListItem> ComputerOptions
        {
            get
            {
                return ComputersList.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Make
                }).ToList();
            }
        }
        public List<SelectListItem> AddTrainingOption
        {
            get
            {
                return AddTrainingProgramList.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();
            }
        }
        public List<SelectListItem> RemoveTrainingOption
        {
            get
            {
                return EnrolledTrainingProgramsList.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }).ToList();
            }
        }
    }   
}
