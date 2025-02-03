namespace SEConnect.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblIntegration
    {
        [Key]
        public int integrationId { get; set; }

        [Required]
        [StringLength(250)]
        public string integrationName { get; set; }

        public int connector1 { get; set; }

        public int connector2 { get; set; }

        [StringLength(50)]
        public string createdBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? createdDate { get; set; }

        public bool? isActive { get; set; }

        [StringLength(50)]
        public string modifiedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? modifiedDate { get; set; }
    }
}
