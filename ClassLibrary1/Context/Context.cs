using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
//using SEConnect.Entities.Customer;
//using SEConnect.Entities.Geographies;

namespace SEConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=dbEntities")
        {

        }
       // public DbSet<Connector> Connectors { get; set; }
        // public DbSet<Customer> Customers { get; set; }
        // public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}