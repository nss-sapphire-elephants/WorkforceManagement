﻿//Author: Nick Hansen
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public int DepartmentId { get; set; }
        public bool IsSupervisor { get; set; }
        [DisplayName("Department")]
        public Department department { get; set; }
        public TrainingProgram trainingProgram { get; set; }
        [DisplayName("Computer")]
        public Computer computer { get; set; }
        [DisplayName("Training Programs")]
        public List<TrainingProgram> trainingPrograms { get; set; }
    }
}