//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ApplicationPermissionValidator.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Linq;

    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;

    #endregion

    /// <summary>
    /// Class ApplicationPermissionValidator.
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Validators.IEntityBusinessRuleValidator" />
    public class ApplicationPermissionValidator : IEntityBusinessRuleValidator
    {
        /// <summary>
        /// The ApplicationModule repository
        /// </summary>
        private readonly IApplicationModuleRepository applicationModuleRepository;

        /// <summary>
        /// The application permission repository
        /// </summary>
        private readonly IApplicationPermissionRepository applicationpermissionRepository;

        /// <summary>
        /// The result
        /// </summary>
        private EntityValidationResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPermissionValidator" /> class.
        /// </summary>
        /// <param name="applicationpermissionRepository">The application permission repository.</param>
        /// <param name="applicationModuleRepository">The application module repository.</param>
        public ApplicationPermissionValidator(
            IApplicationPermissionRepository applicationpermissionRepository,
            IApplicationModuleRepository applicationModuleRepository)
        {
            this.applicationpermissionRepository = applicationpermissionRepository;
            this.applicationModuleRepository = applicationModuleRepository;
            Validator.ArgumentNotNull(applicationpermissionRepository, nameof(IApplicationPermissionRepository));
            Validator.ArgumentNotNull(applicationModuleRepository, nameof(IApplicationModuleRepository));
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
            ApplicationPermission applicationpermission = entity as ApplicationPermission;
            Validator.ArgumentNotNull(applicationpermission, nameof(ApplicationPermission));
            Validator.ArgumentNotNull(rowActionType, nameof(rowActionType));

            this.result = new EntityValidationResult();
            var warningMessages = new List<string>();

            this.PerformApplicationPermissionValidation(applicationpermission, rowActionType, warningMessages);

            this.result.IsValid = warningMessages.Count == 0;
            this.result.ErrorMessage = this.result.IsValid
                                           ? string.Empty
                                           : warningMessages.Where(x => !string.IsNullOrEmpty(x))
                                               .Aggregate((x, y) => x + "," + y);
            return this.result;
        }

        /// <summary>
        /// Applications the perm validation for delete.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="existingapplicationperm">The existing application permission.</param>
        /// <param name="applicationperm">The application permission.</param>
        /// <param name="warningMessages">The warning messages.</param>
        /// <returns>Application Permission.</returns>
        private ApplicationPermission ApplicationPermValidationForDelete(
            string actionType,
            ApplicationPermission existingapplicationperm,
            ApplicationPermission applicationperm,
            List<string> warningMessages)
        {
            if (actionType.Equals(Constants.DeleteOperationIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                if (existingapplicationperm == null)
                {
                    warningMessages.Add(Constants.PermissionDeleteWarningMessage);
                }
                else
                {
                    return existingapplicationperm;
                }
            }

            return applicationperm;
        }

        /// <summary>
        /// Applications the perm validation for insert.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="isEntityExist">if set to <c>true</c> [is entity exist].</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void ApplicationPermValidationForInsert(
            string actionType,
            bool isEntityExist,
            List<string> warningMessages)
        {
            if (actionType.Equals(Constants.InsertOperationIdentifier, StringComparison.InvariantCultureIgnoreCase)
                && isEntityExist)
            {
                warningMessages.Add(Constants.PermissionInsertWarningMessage);
            }
        }

        /// <summary>
        /// Applications the perm validation for update.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="isEntityExist">if set to <c>true</c> [is entity exist].</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void ApplicationPermValidationForUpdate(
            string actionType,
            bool isEntityExist,
            List<string> warningMessages)
        {
            if (actionType.Equals(Constants.UpdateOperationIdentifier, StringComparison.InvariantCultureIgnoreCase)
                && !isEntityExist)
            {
                warningMessages.Add(Constants.PermissionUpdateWarningMessage);
            }
        }

        /// <summary>
        /// Performs the application permission validation.
        /// </summary>
        /// <param name="applicationpermission">The application permission.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void PerformApplicationPermissionValidation(
            ApplicationPermission applicationpermission,
            string actionType,
            List<string> warningMessages)
        {
            bool isEntityExist = false;
            ApplicationPermission existingapplicationPerm =
                this.applicationpermissionRepository.FindBy(
                    ap =>
                        CommonHelpers.IsEqualsWithIgnoreCase(ap.PermissionName, applicationpermission.PermissionName)
                        && ap.ApplicationPermissionId == applicationpermission.ApplicationPermissionId
                        && ap.IsDeleted != true).FirstOrDefault();
            isEntityExist = existingapplicationPerm == null ? false : true;
            applicationpermission.ApplicationPermissionId = isEntityExist
                                                                ? existingapplicationPerm.ApplicationPermissionId
                                                                : Constants.UndeleteEntityState;
            this.ApplicationPermValidationForInsert(actionType, isEntityExist, warningMessages);

            this.ApplicationPermValidationForUpdate(actionType, isEntityExist, warningMessages);

            applicationpermission = this.ApplicationPermValidationForDelete(
                actionType,
                existingapplicationPerm,
                applicationpermission,
                warningMessages);
            this.PermissionNameExistenceCheck(actionType, applicationpermission, warningMessages);
        }

        /// <summary>
        /// Permissions the name existence check.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="applicationperm">The application permission.</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void PermissionNameExistenceCheck(
            string actionType,
            ApplicationPermission applicationperm,
            List<string> warningMessages)
        {
            if ((actionType.Equals(Constants.InsertOperationIdentifier, StringComparison.InvariantCultureIgnoreCase)
                 || actionType.Equals(Constants.UpdateOperationIdentifier, StringComparison.InvariantCultureIgnoreCase))
                && this.applicationpermissionRepository.FindBy(
                        Ap =>
                            Ap.ApplicationPermissionId != applicationperm.ApplicationPermissionId
                            && CommonHelpers.IsEqualsWithIgnoreCase(Ap.PermissionName, applicationperm.PermissionName))
                    .Count() > 0)
            {
                warningMessages.Add(Constants.PermissionInsertWarningMessage);
            }

            if (!(applicationperm.ApplicationModule == null || applicationperm.ApplicationModuleId == 0)
                && this.applicationModuleRepository.FindBy(
                    a => a.ApplicationModuleId == applicationperm.ApplicationModuleId).Count() == 0)
            {
                warningMessages.Add(Constants.ModuleIDNotExistingWarningMessage);
            }
        }
    }
}