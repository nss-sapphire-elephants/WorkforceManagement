/*
 * CREATED BY HM
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime Purchased = new DateTime(2000,1,1);

        [Required]
        public DateTime? Assigned { get; set; }
    }
}
