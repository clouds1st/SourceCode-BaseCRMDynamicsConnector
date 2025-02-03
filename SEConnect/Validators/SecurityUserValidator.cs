//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="SecurityUserValidator.cs" company="Microsoft">
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//  OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------- 

namespace HP.ICE.BusinessLogic.Validators
{
    #region Namespaces

    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.Extensions;

    #endregion

    /// <summary>
    /// Class SecurityUserValidator.
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Validators.IEntityBusinessRuleValidator" />
    public class SecurityUserValidator : IEntityBusinessRuleValidator
    {
        /// <summary>
        /// The metric item repository
        /// </summary>
        private readonly IApplicationUserRepository applicationuserRepository;

        /// <summary>
        /// The business area group repository
        /// </summary>
        private readonly IBusinessAreaGroupRepository businessAreaGroupRepository;

        /// <summary>
        /// The business area hierarchy repository
        /// </summary>
        private readonly IBusinessAreaHierarchyRepository businessAreaHierarchyRepository;

        /// <summary>
        /// The reference value repository
        /// </summary>
        private readonly IReferenceValuesRepository referenceValueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityUserValidator"/> class.
        /// </summary>
        /// <param name="referenceValueRepository">The reference value repository.</param>
        /// <param name="businessAreaHierarchyRepository">The business area hierarchy repository.</param>
        /// <param name="businessAreaGrouprepository">The business area group repository.</param>
        /// <param name="applicationuserRepository">The application user repository.</param>
        public SecurityUserValidator(
            IReferenceValuesRepository referenceValueRepository,
            IBusinessAreaHierarchyRepository businessAreaHierarchyRepository,
            IBusinessAreaGroupRepository businessAreaGrouprepository,
            IApplicationUserRepository applicationuserRepository)
        {
            Validator.ArgumentNotNull(referenceValueRepository, nameof(referenceValueRepository));
            Validator.ArgumentNotNull(applicationuserRepository, nameof(applicationuserRepository));
            Validator.ArgumentNotNull(businessAreaHierarchyRepository, nameof(businessAreaHierarchyRepository));

            this.referenceValueRepository = referenceValueRepository;
            this.applicationuserRepository = applicationuserRepository;
            this.businessAreaHierarchyRepository = businessAreaHierarchyRepository;
            this.businessAreaGroupRepository = businessAreaGrouprepository;
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="rowActionType">Type of the row action.</param>
        /// <returns>Returns entity validation result</returns>
        public EntityValidationResult ValidateEntity<T>(T entity, string rowActionType) where T : class, new()
        {
            ApplicationUser applicationuser = entity as ApplicationUser;
            Validator.ArgumentNotNull(applicationuser, nameof(ApplicationUser));
            Validator.ArgumentNotNull(rowActionType, nameof(rowActionType));

            string message = string.Empty;

            // if ((string.Equals(rowActionType, Constants.InsertOperationIdentifier) || string.Equals(rowActionType, Constants.UpdateOperationIdentifier)))
            // {
            // if (!CommonHelpers.IsEndDateGreaterThanStartDate(applicationuser.StartDate, applicationuser.EndDate))
            // {
            // message = Constants.MetricItemInvalidEffectiveDatesWariningMessage;
            // }
            // else
            // {
            // message = AssignBusinessAreaGroup(metricItem);
            // }
            // }
            if (!string.IsNullOrWhiteSpace(message))
            {
                return new EntityValidationResult { IsValid = false, ErrorMessage = message };
            }

            return new EntityValidationResult();

            // CommonHelpers.IsEqualsWithIgnoreCase(rowActionType, Constants.InsertOperationIdentifier) ? this.ProcessBusinessRulesForMetricItemInsertion(applicationuser) : this.ProcessBusinessRulesForMetricItemModification(applicationuser, rowActionType);
        }
    }
}