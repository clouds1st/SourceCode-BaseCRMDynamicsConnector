namespace SEConnect.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SEConnect : DbContext
    {
        public SEConnect()
            : base("name=SEConnectDBConnection")
        {
        }

        public virtual DbSet<tblConnector> tblConnectors { get; set; }
        public virtual DbSet<tblIntegration> tblIntegrations { get; set; }
        public virtual DbSet<tblUserIntegration> tblUserIntegrations { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<database_firewall_rules>()
                .Property(e => e.start_ip_address)
                .IsUnicode(false);

            modelBuilder.Entity<database_firewall_rules>()
                .Property(e => e.end_ip_address)
                .IsUnicode(false);
        }
    }
}
