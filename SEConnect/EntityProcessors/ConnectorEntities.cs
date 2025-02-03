using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SEConnect.Data;
using SEConnect.Entities.Connector;
using SEConnect.Entities.Customer.ViewModels;


namespace SEConnect.Business.EntityProcessors
{
    public class ConnectorEntities
    {
        public List<ConnectorDisplayViewModel> GetConnectors()
        {
            try
            {


                SEConnect.Data.Repositories.ConnectorRepository repositories = new SEConnect.Data.Repositories.ConnectorRepository();
                List<tblConnector> connectorsList = new List<tblConnector>();
                connectorsList = repositories.GetConnectors();

                if (connectorsList != null)
                {
                    List<ConnectorDisplayViewModel> connectorDisplays = new List<ConnectorDisplayViewModel>();
                    foreach (var c in connectorsList)
                    {
                        var connectorDisplay = new ConnectorDisplayViewModel()
                        {
                            ConnectorId = c.connectorId,
                            ConnectorName = c.connectorName
                        };
                        connectorDisplays.Add(connectorDisplay);
                    }
                    return connectorDisplays;
                }
                return null;

            }
            catch (Exception ex)
            {

                throw;
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