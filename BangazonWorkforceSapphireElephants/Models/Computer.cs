/*
 * CREATED BY HM
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models
{
    public class Computer
    {
        public int Id { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DecomissionDate { get; set; }
        [Required]
        [DisplayName("Computer Make")]
        public string Make { get; set; }
        [Required]
        public string Manufacturer { get; set; }

        public Employee Employee { get; set; }
    }
}
