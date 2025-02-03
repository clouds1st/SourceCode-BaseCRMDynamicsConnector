using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SEConnect.Data.Repositories
{
    public class ConnectorRepository
    {
        public List<tblConnector> GetConnectors()
        {
            try
            {
                using (var context = new SEConnect())
                {
                    List<tblConnector> connectors;
                    // return context.Set<User>().Where(u => u.guid == userGUID).FirstOrDefault().projects.OrderBy(x => x.name).ToList<Project>();
                    //connectors = context.tblConnectors.AsNoTracking()
                    //            .Include(x => x.connectorId)
                    //            .Include(x => x.connectorName)
                    //            .ToList();

                    connectors = context.Set<tblConnector>().ToList<tblConnector>();

                    return connectors;
                }
            }
            catch (Exception dbex)
            {

                throw dbex;
            }


        }


        //public CustomerEditViewModel CreateCustomer()
        //{
        //    var cRepo = new CountriesRepository();
        //    var rRepo = new RegionsRepository();
        //    var customer = new CustomerEditViewModel()
        //    {
        //        CustomerID = Guid.NewGuid().ToString(),
        //        Countries = cRepo.GetCountries(),
        //        Regions = rRepo.GetRegions()
        //    };
        //    return customer;
        //}

        //public bool SaveCustomer(CustomerEditViewModel customeredit)
        //{
        //    if (customeredit != null)
        //    {
        //        using (var context = new ApplicationDbContext())
        //        {
        //            if (Guid.TryParse(customeredit.CustomerID, out Guid newGuid))
        //            {
        //                var customer = new Customer()
        //                {
        //                    CustomerID = newGuid,
        //                    CustomerName = customeredit.CustomerName,
        //                    CountryIso3 = customeredit.SelectedCountryIso3,
        //                    RegionCode = customeredit.SelectedRegionCode
        //                };
        //                customer.Country = context.Countries.Find(customeredit.SelectedCountryIso3);
        //                customer.Region = context.Regions.Find(customeredit.SelectedRegionCode);

        //                context.Customers.Add(customer);
        //                context.SaveChanges();
        //                return true;
        //            }
        //        }
        //    }
        //    // Return false if customeredit == null or CustomerID is not a guid
        //    return false;
        //}
    }
}