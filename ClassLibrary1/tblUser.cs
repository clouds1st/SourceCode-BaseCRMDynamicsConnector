namespace SEConnect.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblUser
    {
        [Key]
        public long userId { get; set; }

        [Required]
        [StringLength(100)]
        public string fname { get; set; }

        [StringLength(100)]
        public string lname { get; set; }

        [Required]
        [StringLength(150)]
        public string email { get; set; }

        [StringLength(12)]
        public string userPassword { get; set; }

        [StringLength(100)]
        public string company { get; set; }

        [StringLength(50)]
        public string jobtitle { get; set; }

        public DateTime? createddate { get; set; }

        public DateTime? modifieddate { get; set; }
    }
}
