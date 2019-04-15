using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string Name { get; set; }        

        [Required(ErrorMessage = "Enter the start date and time.")]
        [Display(Name = "Start Date/Time")]        
        public DateTime StartDate { get; set; }          

        [Required(ErrorMessage = "Enter the end date and time.")]
        [Display(Name = "End Date/Time")]        
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Enter the maximum Attendees for this training.")]
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }

        public List<Employee> EmployeeList { get; set; }        
    }
}
