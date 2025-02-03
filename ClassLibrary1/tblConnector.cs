namespace SEConnect.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblConnector
    {
        [Key]
        public int connectorId { get; set; }

        [Required]
        [StringLength(100)]
        public string connectorName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? createdDate { get; set; }

        [StringLength(50)]
        public string createdBy { get; set; }

        public bool? isActive { get; set; }
    }
}
