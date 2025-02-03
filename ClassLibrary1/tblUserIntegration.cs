namespace SEConnect.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblUserIntegration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long userIntegrationId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int integrationId { get; set; }

        [Required]
        [StringLength(50)]
        public string userName { get; set; }

        [StringLength(50)]
        public string createdBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? createdDate { get; set; }

        [StringLength(50)]
        public string modifiedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? modifiedDate { get; set; }

        public bool? isActive { get; set; }
    }
}
