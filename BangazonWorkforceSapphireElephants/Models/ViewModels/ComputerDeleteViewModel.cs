/*
 * CREATED BY HM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class ComputerDeleteViewModel
    {
        public int Id { get; set; }


        public DateTime PurchaseDate { get; set; }


        public string Make { get; set; }


        public string Manufacturer { get; set; }

        public bool DisplayDelete { get; set; }
    }
}
