//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ServiceGroupValidator.cs" company="Microsoft">
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

    using System;
    using System.Linq;
    using System.Text;

    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;

    #endregion

    /// <summary>
    /// The product group validator
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Validators.IEntityBusinessRuleValidator" />
    public class ServiceGroupValidator : ValidatorBase, IEntityBusinessRuleValidator
    {
        /// <summary>
        /// The business area group repository
        /// </summary>
        private readonly IBusinessAreaGroupRepository businessAreaGroupRepository;

        /// <summary>
        /// The reference value repository
        /// </summary>
        private readonly IReferenceValuesRepository referenceValueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceGroupValidator" /> class.
        /// </summary>
        /// <param name="businessAreaGroupRepository">The business area group repository.</param>
        /// <param name="referenceValueRepository">The reference value repository.</param>
        public ServiceGroupValidator(
            IBusinessAreaGroupRepository businessAreaGroupRepository,
            IReferenceValuesRepository referenceValueRepository)
            : base(referenceValueRepository)
        {
            Validator.ArgumentNotNull(businessAreaGroupRepository, nameof(businessAreaGroupRepository));
            Validator.ArgumentNotNull(referenceValueRepository, nameof(referenceValueRepository));

            this.businessAreaGroupRepository = businessAreaGroupRepository;
            this.referenceValueRepository = referenceValueRepository;
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="rowActionType">Type of the row action.</param>
        /// <returns>
        /// Returns entity validation result
        /// </returns>
        public EntityValidationResult ValidateEntity<T>(T entity, string rowActionType) where T : class, new()
        {
            BusinessAreaGroup businessAreaGroup = entity as BusinessAreaGroup;
            Validator.ArgumentNotNull(businessAreaGroup, nameof(businessAreaGroup));
            Validator.ArgumentNotNull(rowActionType, nameof(rowActionType));

            businessAreaGroup.GroupTypeIndicator = BusinessAreaGroupType.ServiceGroup;

            if ((string.Equals(rowActionType, Constants.InsertOperationIdentifier)
                 || string.Equals(rowActionType, Constants.UpdateOperationIdentifier))
                && !CommonHelpers.IsEndDateGreaterThanStartDate(businessAreaGroup.StartDate, businessAreaGroup.EndDate))
            {
                return new EntityValidationResult
                           {
                               IsValid = false,
                               ErrorMessage =
                                   Constants.ServiceGroupInvalidEffectiveDatesWariningMessage
                           };
            }

            if (CommonHelpers.IsEqualsWithIgnoreCase(rowActionType, Constants.InsertOperationIdentifier))
            {
                return this.ProcessBusinessRulesForServiceGroupInsertion(businessAreaGroup);
            }
            else
            {
                return this.ProcessBusinessRulesForServiceGroupModification(businessAreaGroup, rowActionType);
            }
        }

        /// <summary>
        /// Constructs the entity validation result.
        /// </summary>
        /// <param name="businessAreaGroup">The sales model dimension.</param>
        /// <param name="isEntityInDeleteState">if set to <c>true</c> [is entity in delete state].</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns validation result</returns>
        private static EntityValidationResult ConstructEntityValidationResult(
            BusinessAreaGroup businessAreaGroup,
            bool isEntityInDeleteState,
            string message)
        {
            return new EntityValidationResult { IsValid = string.IsNullOrEmpty(message), ErrorMessage = message };
        }

        /// <summary>
        /// Gets the linked incentive plans.
        /// </summary>
        /// <param name="actionInd">The action ind.</param>
        /// <param name="businessAreaGroupId">The business area group identifier.</param>
        /// <returns>Returns error message</returns>
        private string GetAssociatedEntities(string actionInd, long businessAreaGroupId)
        {
            StringBuilder errorMessageBuilder = new StringBuilder();

            string metricItemNames = this.businessAreaGroupRepository.GetLinkedMetricItemNames(businessAreaGroupId);

            string salesMetrics =
                this.businessAreaGroupRepository.GetSalesMetricLinkedToBusinessGroup(businessAreaGroupId);

            string incentivePlans =
                string.Concat(
                    this.businessAreaGroupRepository.GetIncentivePlansLinkedToBusinessGroup(businessAreaGroupId),
                    this.businessAreaGroupRepository.GetRegionalIncentivePlansLinkedToBusinessGroup(businessAreaGroupId));

            if (!string.IsNullOrEmpty(metricItemNames))
            {
                errorMessageBuilder.Append(" Metric Items : ").Append(metricItemNames);
            }

            if (!string.IsNullOrEmpty(salesMetrics))
            {
                errorMessageBuilder.Append(" ,Sales Metrics : ").Append(salesMetrics);
            }

            if (!string.IsNullOrEmpty(incentivePlans))
            {
                errorMessageBuilder.Append(" ,Incentive plans : ").Append(incentivePlans);
            }

            if (errorMessageBuilder.Length > 0)
            {
                return this.GetErrorMessage(actionInd, errorMessageBuilder.ToString().Trim(','));
            }

            return null;
        }

        /// <summary>
        /// Gets the conflicting entity.
        /// </summary>
        /// <param name="businessAreaGroup">The business area group.</param>
        /// <returns>Returns business area group</returns>
        private BusinessAreaGroup GetConflictingEntity(BusinessAreaGroup businessAreaGroup)
        {
            return
                this.businessAreaGroupRepository.FindBy(
                        x =>
                            x.BusinessAreaGroupId != businessAreaGroup.BusinessAreaGroupId
                            && x.BusinessAreaGroupName == businessAreaGroup.BusinessAreaGroupName
                            && x.StartDate == businessAreaGroup.StartDate && x.EndDate == businessAreaGroup.EndDate)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Gets the incentive plan error message.
        /// </summary>
        /// <param name="actionInd">The action ind.</param>
        /// <param name="substitutionParam">The substitution parameter.</param>
        /// <returns>Returns error message</returns>
        private string GetErrorMessage(string actionInd, string substitutionParam)
        {
            if (CommonHelpers.IsEqualsWithIgnoreCase(actionInd, Constants.UpdateOperationIdentifier))
            {
                return string.Format(Constants.IncentivePlanLinkedServiceGroupUpdateWarningMessage, substitutionParam);
            }
            else
            {
                return string.Format(Constants.IncentivePlanLinkedServiceGroupDeleteWarningMessage, substitutionParam);
            }
        }

        /// <summary>
        /// Processes the business rules for product group insertion.
        /// </summary>
        /// <param name="businessAreaGroup">The business area group.</param>
        /// <returns>Returns validation result</returns>
        private EntityValidationResult ProcessBusinessRulesForServiceGroupInsertion(BusinessAreaGroup businessAreaGroup)
        {
            bool isEntityInDeleteState = false;
            string message = null;
            bool isEntityInDraftState = this.IsEntityInDraftState(businessAreaGroup.BusinessAreaGroupStatus);

            if (isEntityInDraftState)
            {
                var existingEntity =
                    this.businessAreaGroupRepository.FindBy(
                        ba =>
                            CommonHelpers.IsEqualsWithIgnoreCase(
                                ba.GroupTypeIndicator,
                                businessAreaGroup.GroupTypeIndicator)
                            && CommonHelpers.IsEqualsWithIgnoreCase(
                                ba.BusinessAreaGroupName,
                                businessAreaGroup.BusinessAreaGroupName)).FirstOrDefault();

                if (existingEntity != null)
                {
                    isEntityInDeleteState = existingEntity?.IsDeleted ?? false;

                    if (!isEntityInDeleteState)
                    {
                        message = Constants.ServiceGroupNameAlreadyUsedMessage;
                    }
                }
            }
            else
            {
                message = Constants.EntityDefaultStateNotValidWarningMessage;
            }

            return ConstructEntityValidationResult(businessAreaGroup, isEntityInDeleteState, message);
        }

        /// <summary>
        /// Processes the business rules for product group modification.
        /// </summary>
        /// <param name="businessAreaGroup">The business area group.</param>
        /// <param name="actionInd">The action  indicator.</param>
        /// <returns>Returns entity validation result</returns>
        private EntityValidationResult ProcessBusinessRulesForServiceGroupModification(
            BusinessAreaGroup businessAreaGroup,
            string actionInd)
        {
            string message = null;
            EntityValidationResult entityValidationResult = new EntityValidationResult();
            var existingBusinessAreaGroup =
                this.businessAreaGroupRepository.FindBy(
                        x => x.BusinessAreaGroupId == Convert.ToInt32(businessAreaGroup.BusinessAreaGroupId))
                    .FirstOrDefault();

            if (existingBusinessAreaGroup == null || Convert.ToBoolean(existingBusinessAreaGroup.IsDeleted))
            {
                message = Constants.ServiceGroupUpdateWarningMessage;
            }
            else
            {
                string associatedEntites = this.GetAssociatedEntities(actionInd, businessAreaGroup.BusinessAreaGroupId);

                if (!string.IsNullOrEmpty(associatedEntites))
                {
                    message = associatedEntites;
                }
                else if (CommonHelpers.IsEqualsWithIgnoreCase(actionInd, Constants.UpdateOperationIdentifier))
                {
                    var conflictingEntity = this.GetConflictingEntity(businessAreaGroup);

                    if (conflictingEntity != null)
                    {
                        if (!conflictingEntity.IsDeleted)
                        {
                            message = Constants.ServiceGroupConflictingEntityWarningMessage;
                        }
                    }
                }
            }

            entityValidationResult.IsValid = string.IsNullOrEmpty(message);
            entityValidationResult.ErrorMessage = message;
            return entityValidationResult;
        }
    }
}