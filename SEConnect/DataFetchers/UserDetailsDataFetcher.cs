// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDetailsDataFetcher.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   //   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   //   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   //   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   //   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   //   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   //   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.DataFetchers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.ExcelDownload;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;

    #endregion

    /// <summary>
    /// User Details data fetcher
    /// </summary>
    /// <seealso cref="IExcelDataFetcher" />
    public class UserDetailsDataFetcher : IExcelDataFetcher
    {
        /// <summary>
        /// The user management logic
        /// </summary>
        private readonly IUserManagementLogic userManagementLogic;

        /// <summary>
        /// The application user repository
        /// </summary>
        private readonly IApplicationUserRepository applicationuserrepository;

        /// <summary>
        /// The application user role repository
        /// </summary>
        private readonly IApplicationUserRoleRepository applicationuserrolerepository;

        /// <summary>
        /// The application user sale org repository
        /// </summary>
        private readonly IApplicationUserSalesOrgRepository applicationuserSalesOrgrepository;

        /// <summary>
        /// The application user country assignment repository
        /// </summary>
        private readonly IApplicationUserCountryAssignmentRepository applicationuserCountryAssigrepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetailsDataFetcher"/> class. 
        /// </summary>
        /// <param name="userManagementLogic">
        /// The User Management logic.
        /// </param>
        /// <param name="applicationuserrepository">
        /// The Application User Repository.
        /// </param>
        /// <param name="applicationuserrolerepository">
        /// The Application User Role Repository.
        /// </param>
        /// <param name="applicationuserSalesOrgrepository">
        /// The Application User Sales Org Repository.
        /// </param>
        /// <param name="applicationuserCountryAssigrepository">
        /// The Application User Country Assignment Repository.
        /// </param>
        public UserDetailsDataFetcher(
            IUserManagementLogic userManagementLogic,
            IApplicationUserRepository applicationuserrepository,
            IApplicationUserRoleRepository applicationuserrolerepository,
            IApplicationUserSalesOrgRepository applicationuserSalesOrgrepository,
            IApplicationUserCountryAssignmentRepository applicationuserCountryAssigrepository)
        {
            Validator.ArgumentNotNull(userManagementLogic, nameof(IUserManagementLogic));
            Validator.ArgumentNotNull(applicationuserrepository, nameof(IApplicationUserRepository));
            Validator.ArgumentNotNull(applicationuserrolerepository, nameof(IApplicationUserRoleRepository));
            Validator.ArgumentNotNull(applicationuserSalesOrgrepository, nameof(IApplicationUserSalesOrgRepository));
            Validator.ArgumentNotNull(
                applicationuserCountryAssigrepository,
                nameof(IApplicationUserCountryAssignmentRepository));
            this.userManagementLogic = userManagementLogic;
            this.applicationuserrepository = applicationuserrepository;
            this.applicationuserrolerepository = applicationuserrolerepository;
            this.applicationuserSalesOrgrepository = applicationuserSalesOrgrepository;
            this.applicationuserCountryAssigrepository = applicationuserCountryAssigrepository;
        }

        /// <summary>
        /// Gets the data for excel generation.
        /// </summary>
        /// <param name="dataFetcherParameters">The data fetcher parameters.</param>
        /// <returns>
        /// Returns data source for each sheet
        /// </returns>
        public IReadOnlyDictionary<string, IQueryable<object>> GetDataForExcelGeneration(
            DataFetcherParameters dataFetcherParameters)
        {
            Validator.ArgumentNotNull(dataFetcherParameters, nameof(DataFetcherParameters));
            Validator.ArgumentNotNull(
                dataFetcherParameters.DownloadRequest,
                nameof(dataFetcherParameters.DownloadRequest));
            var searchCriteria =
                CommonHelpers.DeserializeHelper<UserManagementSearchCriteria>(
                    dataFetcherParameters.DownloadRequest.AdditionalSearchCriteria);
            var usermanagementGridDataCollection = this.userManagementLogic.GetUserGridWithSearchCriteria(
                searchCriteria);
            var filteredListUserGridDataCollection =
                dataFetcherParameters.DownloadRequest.DataSourceRequest.ApplyQueryExpression(
                    usermanagementGridDataCollection);
            var userids = filteredListUserGridDataCollection.Select(x => x.ApplicationUserId);
            var userdetails =
                this.applicationuserrepository.GetAll()
                    .Where(
                        x =>
                            userids.Contains(x.ApplicationUserId)
                            && (dataFetcherParameters.DownloadRequest.IsIncludeDeletedRecords || x.IsDeleted == false));

            var applicationUserRoleDetails =
                from userroledetails in
                dataFetcherParameters.DownloadRequest.IsIncludeDeletedRecords
                    ? this.applicationuserrolerepository.GetAll()
                    : this.applicationuserrolerepository.Get(x => !x.IsDeleted)
                where userids.Contains(userroledetails.ApplicationUserId)
                select userroledetails;

            var applicationUserSalesOrgDetails =
                from usersalesorg in
                dataFetcherParameters.DownloadRequest.IsIncludeDeletedRecords
                    ? this.applicationuserSalesOrgrepository.Get(null, null, x => x.SalesOrganization)
                    : this.applicationuserSalesOrgrepository.Get(x => !x.IsDeleted, null, y => y.SalesOrganization)
                where userdetails.Select(x => x.ApplicationUserId).Contains(usersalesorg.ApplicationUserId)
                where userids.Contains(usersalesorg.ApplicationUserId)
                select
                new ApplicationUserSalesOrg
                    {
                        ApplicationUserSalesOrgId = usersalesorg.ApplicationUserSalesOrgId,
                        ApplicationUserId = usersalesorg.ApplicationUserId,
                        SalesOrganizationId =
                            Convert.ToInt64(
                                usersalesorg.SalesOrganization.SalesOrganizationNumber),
                        IsDeleted = usersalesorg.IsDeleted,
                        ModifiedDate = usersalesorg.ModifiedDate,
                        ModifiedBy = usersalesorg.ModifiedBy,
                        CreatedDate = usersalesorg.CreatedDate,
                        CreatedBy = usersalesorg.CreatedBy,
                    };

            var applicationUserCountryAssigDetails =
                from usercountryassig in
                dataFetcherParameters.DownloadRequest.IsIncludeDeletedRecords
                    ? this.applicationuserCountryAssigrepository.GetAll()
                    : this.applicationuserCountryAssigrepository.Get(x => !x.IsDeleted)
                where userids.Contains(usercountryassig.ApplicationUserId)
                select usercountryassig;

            return new Dictionary<string, IQueryable<object>>
                       {
                           {
                               ExcelSheetUniqueIdentifiers.ApplicationUser,
                               userdetails
                           },
                           {
                               ExcelSheetUniqueIdentifiers.ApplicationUserRole,
                               applicationUserRoleDetails
                           },
                           {
                               ExcelSheetUniqueIdentifiers.ApplicationUserSalesOrg,
                               applicationUserSalesOrgDetails
                           },
                           {
                               ExcelSheetUniqueIdentifiers
                                   .ApplicationUserCountryAssig,
                               applicationUserCountryAssigDetails
                           },
                       };
        }
    }
}