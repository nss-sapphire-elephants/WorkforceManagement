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
        public string Manufacturer { get; set; }
        public string Make { get; set; }
        [Required]
        public DateTime Purchased { get; set; } 
        public DateTime? Assigned { get; set; }
    }
}
