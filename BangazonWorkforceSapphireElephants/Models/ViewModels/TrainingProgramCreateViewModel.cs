//      -- Created by Colleen Woolsey

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class TrainingProgramCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        public TrainingProgramCreateViewModel()
        {
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;
        }     

        [Required(ErrorMessage = "Enter the start date AND time.")]
        [Display(Name = "Start Date/Time")]
        [DataType(DataType.DateTime)]        
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mi:ss t}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Enter the end date AND time.")]
        [Display(Name = "End Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mi:ss t}", ApplyFormatInEditMode = true)]        
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Enter the maximum Attendees for this training.")]
        public int? MaxAttendees { get; set; }
        
    }
}
