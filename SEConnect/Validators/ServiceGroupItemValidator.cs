//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ServiceGroupItemValidator.cs" company="Microsoft">
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

    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;

    #endregion

    /// <summary>
    /// The service group validator
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Validators.IEntityBusinessRuleValidator" />
    public class ServiceGroupItemValidator : IEntityBusinessRuleValidator
    {
        /// <summary>
        /// The business area group repository
        /// </summary>
        private readonly IBusinessAreaGroupItemRepository businessAreaGroupItemRepository;

        /// <summary>
        /// The business area group item repository
        /// </summary>
        private readonly IBusinessAreaGroupRepository businessAreaGroupRepository;

        /// <summary>
        /// The business area hierarchy repository
        /// </summary>
        private readonly IBusinessAreaHierarchyRepository businessAreaHierarchyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceGroupItemValidator" /> class.
        /// </summary>
        /// <param name="businessAreaGroupItemRepository">The business area group item repository.</param>
        /// <param name="businessAreaGroupRepository">The business area group repository.</param>
        /// <param name="businessAreaHierarchyRepository">The business area hierarchy repository.</param>
        public ServiceGroupItemValidator(
            IBusinessAreaGroupItemRepository businessAreaGroupItemRepository,
            IBusinessAreaGroupRepository businessAreaGroupRepository,
            IBusinessAreaHierarchyRepository businessAreaHierarchyRepository)
        {
            Validator.ArgumentNotNull(businessAreaGroupItemRepository, nameof(businessAreaGroupItemRepository));
            Validator.ArgumentNotNull(businessAreaHierarchyRepository, nameof(businessAreaHierarchyRepository));
            Validator.ArgumentNotNull(businessAreaGroupRepository, nameof(businessAreaGroupRepository));

            this.businessAreaGroupItemRepository = businessAreaGroupItemRepository;
            this.businessAreaHierarchyRepository = businessAreaHierarchyRepository;
            this.businessAreaGroupRepository = businessAreaGroupRepository;
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
            BusinessAreaGroupItem businessAreaGroupItem = entity as BusinessAreaGroupItem;
            Validator.ArgumentNotNull(businessAreaGroupItem, nameof(BusinessAreaGroupItem));
            Validator.ArgumentNotNull(rowActionType, nameof(rowActionType));

            businessAreaGroupItem.ServiceGroupProductTypeInd = BusinessAreaGroupType.ServiceGroup;

            if ((string.Equals(rowActionType, Constants.InsertOperationIdentifier)
                 || string.Equals(rowActionType, Constants.UpdateOperationIdentifier))
                && (!this.businessAreaGroupRepository.FindBy(
                            bg =>
                                bg.BusinessAreaGroupId == businessAreaGroupItem.BusinessAreaGroupId && bg.IsDeleted != true)
                        .Any()))
            {
                return new EntityValidationResult
                           {
                               IsValid = false,
                               ErrorMessage = Constants.ServiceGroupItemNonExistenceWarningMessage
                           };
            }

            if (CommonHelpers.IsEqualsWithIgnoreCase(rowActionType, Constants.InsertOperationIdentifier))
            {
                return this.ProcessBusinessRulesForServiceGroupItemInsertion(businessAreaGroupItem);
            }
            else
            {
                return this.ProcessBusinessRulesForServiceGroupItemModification(businessAreaGroupItem, rowActionType);
            }
        }

        /// <summary>
        /// Gets the business area hierarchy identifier.
        /// </summary>
        /// <param name="businessAreaGroupId">The business area group identifier.</param>
        /// <param name="businessAreaCode">The business area code.</param>
        /// <returns>Returns hierarchy</returns>
        private int? GetBusinessAreaHierarchyId(long businessAreaGroupId, string businessAreaCode)
        {
            var businessAreaHierarchy = this.businessAreaHierarchyRepository.GetBusinessAreaHierarchy(businessAreaCode);

            if (businessAreaHierarchy != null)
            {
                return businessAreaHierarchy.BusinessAreaHierarchyId;
            }

            return null;
        }

        /// <summary>
        /// Gets the existing business area group item.
        /// </summary>
        /// <param name="businessAreaGroupItem">The business area group item.</param>
        /// <returns>Returns entity</returns>
        private BusinessAreaGroupItem GetExistingBusinessAreaGroupItem(BusinessAreaGroupItem businessAreaGroupItem)
        {
            return
                this.businessAreaGroupItemRepository.FindBy(
                        x =>
                            x.BusinessAreaGroupId == businessAreaGroupItem.BusinessAreaGroupId
                            && x.BusinessAreaHierarchyId == businessAreaGroupItem.BusinessAreaHierarchyId
                            && x.HierarchyLevel == businessAreaGroupItem.HierarchyLevel
                            && x.Code == businessAreaGroupItem.Code
                            && x.ServiceGroupProductTypeInd == businessAreaGroupItem.ServiceGroupProductTypeInd)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Processes the business rules for product group insertion.
        /// </summary>
        /// <param name="businessAreaGroupItem">The business area group.</param>
        /// <returns>Returns validation result</returns>
        private EntityValidationResult ProcessBusinessRulesForServiceGroupItemInsertion(
            BusinessAreaGroupItem businessAreaGroupItem)
        {
            bool isEntityInDeleteState = false;
            string message = null;
            BusinessAreaGroupItem existingEntity = this.GetExistingBusinessAreaGroupItem(businessAreaGroupItem);

            if (existingEntity != null)
            {
                isEntityInDeleteState = existingEntity?.IsDeleted ?? false;

                if (!isEntityInDeleteState)
                {
                    message = Constants.DuplicateServiceGroupItemInsertionWarningMessage;
                }
            }
            else
            {
                businessAreaGroupItem.BusinessAreaHierarchyId =
                    this.GetBusinessAreaHierarchyId(
                        businessAreaGroupItem.BusinessAreaGroupId,
                        businessAreaGroupItem.BusinessAreaCode);
            }

            return new EntityValidationResult { IsValid = string.IsNullOrEmpty(message), ErrorMessage = message };
        }

        /// <summary>
        /// Processes the business rules for product group modification.
        /// </summary>
        /// <param name="businessAreaGroupItem">The business area group.</param>
        /// <param name="actionInd">The action  indicator.</param>
        /// <returns>Returns entity validation result</returns>
        private EntityValidationResult ProcessBusinessRulesForServiceGroupItemModification(
            BusinessAreaGroupItem businessAreaGroupItem,
            string actionInd)
        {
            string message = null;
            EntityValidationResult entityValidationResult = new EntityValidationResult();
            var existingBusinessAreaGroupItem =
                this.businessAreaGroupItemRepository.FindBy(
                    x => x.BusinessAreaGroupItemId == businessAreaGroupItem.BusinessAreaGroupItemId).FirstOrDefault();

            if (existingBusinessAreaGroupItem == null || Convert.ToBoolean(existingBusinessAreaGroupItem.IsDeleted))
            {
                message = Constants.ServiceGroupItemNonExistenceWarningMessage;
            }
            else if (CommonHelpers.IsEqualsWithIgnoreCase(actionInd, Constants.UpdateOperationIdentifier))
            {
                businessAreaGroupItem.BusinessAreaHierarchyId =
                    this.GetBusinessAreaHierarchyId(
                        businessAreaGroupItem.BusinessAreaGroupId,
                        businessAreaGroupItem.BusinessAreaCode);
                var conflictingEntity =
                    this.businessAreaGroupItemRepository.FindBy(
                        x =>
                            x.BusinessAreaGroupItemId != existingBusinessAreaGroupItem.BusinessAreaGroupItemId
                            && x.BusinessAreaGroupId == existingBusinessAreaGroupItem.BusinessAreaGroupId
                            && x.BusinessAreaHierarchyId == existingBusinessAreaGroupItem.BusinessAreaHierarchyId
                            && x.HierarchyLevel == existingBusinessAreaGroupItem.HierarchyLevel
                            && x.Code == existingBusinessAreaGroupItem.Code).FirstOrDefault();

                if (conflictingEntity != null)
                {
                    if (!Convert.ToBoolean(conflictingEntity.IsDeleted))
                    {
                        message = Constants.ServiceGroupItemModificationWarningMessage;
                    }
                }
            }

            entityValidationResult.IsValid = string.IsNullOrEmpty(message);
            entityValidationResult.ErrorMessage = message;
            return entityValidationResult;
        }
    }
}