
namespace SEConnect.Entities.Connector
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   public class Connector
    {
        [Key]
        [Column(Order = 1)]
        public int ConnectorID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ConnectorName { get; set; }

        //[Required]
        //[MaxLength(3)]
        //public string CountryIso3 { get; set; }

        //[MaxLength(3)]
        //public string RegionCode { get; set; }

        //public virtual Country Country { get; set; }

        //public virtual Region Region { get; set; }
    }
}


