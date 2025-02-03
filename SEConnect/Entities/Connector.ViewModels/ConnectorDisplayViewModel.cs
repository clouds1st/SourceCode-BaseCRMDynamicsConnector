using System;
using System.ComponentModel.DataAnnotations;

namespace SEConnect.Entities.Customer.ViewModels
{
    public class ConnectorDisplayViewModel
    {
        [Display(Name = "Customer Number")]
        public int ConnectorId { get; set; }

        [Display(Name = "Connector Name")]
        public string ConnectorName { get; set; }

        //[Display(Name = "Country")]
        //public string CountryName { get; set; }

        //[Display(Name = "State / Province / Region")]
        //public string RegionName { get; set; }
    }
}