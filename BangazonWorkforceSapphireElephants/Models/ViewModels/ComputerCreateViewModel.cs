using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceSapphireElephants.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public string Manufacturer { get; set; }
        public string Make { get; set; }
        public DateTime Purchased { get; set; }
        public DateTime? Assigned { get; set; }
    }
}
