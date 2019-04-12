﻿//Author: Nick Hansen
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
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
        public Department department { get; set; }
        public TrainingProgram trainingProgram { get; set; }
        public Computer computer { get; set; }
        public List<TrainingProgram> trainingPrograms { get; set; }
    }
}