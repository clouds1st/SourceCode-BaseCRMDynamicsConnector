//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ApplicationRolePermissionValidator.cs" company="Microsoft">
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

    #endregion

    /// <summary>
    /// Class ApplicationRolePermissionValidator.
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Validators.IEntityBusinessRuleValidator" />
    public class ApplicationRolePermissionValidator : IEntityBusinessRuleValidator
    {
        /// <summary>
        /// The Application Permission repository
        /// </summary>
        private readonly IApplicationPermissionRepository applicationPermissionRepository;

        /// <summary>
        /// The ApplicationRole repository
        /// </summary>
        private readonly IApplicationRoleRepository applicationRoleRepository;

        /// <summary>
        /// The application role permission repository
        /// </summary>
        private readonly IApplicationRolePermissionRepository appRolePermrepository;

        /// <summary>
        /// The result
        /// </summary>
        private EntityValidationResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRolePermissionValidator" /> class.
        /// </summary>
        /// <param name="appRolePermrepository">The application role permission repository.</param>
        /// <param name="applicationRoleRepository">The application role repository.</param>
        /// <param name="applicationPermissionRepository">The application permission repository.</param>
        public ApplicationRolePermissionValidator(
            IApplicationRolePermissionRepository appRolePermrepository,
            IApplicationRoleRepository applicationRoleRepository,
            IApplicationPermissionRepository applicationPermissionRepository)
        {
            this.appRolePermrepository = appRolePermrepository;
            this.applicationRoleRepository = applicationRoleRepository;
            this.applicationPermissionRepository = applicationPermissionRepository;
            Validator.ArgumentNotNull(appRolePermrepository, nameof(IApplicationRolePermissionRepository));
            Validator.ArgumentNotNull(applicationRoleRepository, nameof(IApplicationRoleRepository));
            Validator.ArgumentNotNull(applicationPermissionRepository, nameof(IApplicationPermissionRepository));
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
            ApplicationRolePermission applicationroleperm = entity as ApplicationRolePermission;
            Validator.ArgumentNotNull(applicationroleperm, nameof(ApplicationRolePermission));
            Validator.ArgumentNotNull(rowActionType, nameof(rowActionType));

            this.result = new EntityValidationResult();
            var warningMessages = new List<string>();

            this.PerformApplicationRolePermValidation(applicationroleperm, rowActionType, warningMessages);

            this.result.IsValid = warningMessages.Count == 0;
            this.result.ErrorMessage = this.result.IsValid
                                           ? string.Empty
                                           : warningMessages.Where(x => !string.IsNullOrEmpty(x))
                                               .Aggregate((x, y) => x + "," + y);
            return this.result;
        }

        /// <summary>
        /// The app role perm existence check.
        /// </summary>
        /// <param name="actionType">
        /// The action type.
        /// </param>
        /// <param name="applicationroleperm">
        /// The application role permission.
        /// </param>
        /// <param name="warningMessages">
        /// The warning messages.
        /// </param>
        private void AppRolePermExistenceCheck(
            string actionType,
            ApplicationRolePermission applicationroleperm,
            List<string> warningMessages)
        {
            if (!(applicationroleperm.ApplicationRole == null || applicationroleperm.ApplicationRoleId == 0)
                && this.applicationRoleRepository.FindBy(
                    role => role.ApplicationRoleId == applicationroleperm.ApplicationRoleId).Count() == 0)
            {
                warningMessages.Add(Constants.RoleIDNotExistingWarningMessage);
            }

            if (!(applicationroleperm.ApplicationPermission == null || applicationroleperm.ApplicationPermissionId == 0)
                && this.applicationPermissionRepository.FindBy(
                    role => role.ApplicationPermissionId == applicationroleperm.ApplicationPermissionId).Count() == 0)
            {
                warningMessages.Add(Constants.PermissionIDNotExistingWarningMessage);
            }
        }

        /// <summary>
        /// Applications the role perm validation for insert.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="isEntityExist">if set to <c>true</c> [is entity exist].</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void AppRolePermValidationForInsert(string actionType, bool isEntityExist, List<string> warningMessages)
        {
            if (actionType.Equals(Constants.InsertOperationIdentifier, StringComparison.InvariantCultureIgnoreCase)
                && isEntityExist)
            {
                warningMessages.Add(Constants.RolePermInsertWarningMessage);
            }
        }

        /// <summary>
        /// Applications the role perm validation for update.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="isEntityExist">if set to <c>true</c> [is entity exist].</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void AppRolePermValidationForUpdate(string actionType, bool isEntityExist, List<string> warningMessages)
        {
            if (actionType.Equals(Constants.UpdateOperationIdentifier, StringComparison.InvariantCultureIgnoreCase)
                && !isEntityExist)
            {
                warningMessages.Add(Constants.RolePermUpdateWarningMessage);
            }
        }

        /// <summary>
        /// Applications the user role validation for delete.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="existingapproleperm">The existing application role permission.</param>
        /// <param name="applicationroleperm">The application role permission.</param>
        /// <param name="warningMessages">The warning messages.</param>
        /// <returns>Application Role Permission.</returns>
        private ApplicationRolePermission AppUserRoleValidationForDelete(
            string actionType,
            ApplicationRolePermission existingapproleperm,
            ApplicationRolePermission applicationroleperm,
            List<string> warningMessages)
        {
            if (actionType.Equals(Constants.DeleteOperationIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                if (existingapproleperm == null)
                {
                    warningMessages.Add(Constants.RolePermDeleteWarningMessage);
                }
                else
                {
                    return existingapproleperm;
                }
            }

            return applicationroleperm;
        }

        /// <summary>
        /// Performs the application role perm validation.
        /// </summary>
        /// <param name="applicationroleperm">The application role permission.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="warningMessages">The warning messages.</param>
        private void PerformApplicationRolePermValidation(
            ApplicationRolePermission applicationroleperm,
            string actionType,
            List<string> warningMessages)
        {
            bool isEntityExist = false;
            ApplicationRolePermission existingapproleperm =
                this.appRolePermrepository.FindBy(
                        x =>
                            x.ApplicationPermissionId == applicationroleperm.ApplicationPermissionId
                            && x.ApplicationRoleId == applicationroleperm.ApplicationRoleId && x.IsDeleted != true)
                    .FirstOrDefault();
            isEntityExist = existingapproleperm == null ? false : true;
            applicationroleperm.ApplicationRolePermissionId = isEntityExist
                                                                  ? existingapproleperm.ApplicationRolePermissionId
                                                                  : Constants.UndeleteEntityState;
            this.AppRolePermValidationForInsert(actionType, isEntityExist, warningMessages);

            this.AppRolePermValidationForUpdate(actionType, isEntityExist, warningMessages);

            this.AppUserRoleValidationForDelete(actionType, existingapproleperm, applicationroleperm, warningMessages);
            this.AppRolePermExistenceCheck(actionType, applicationroleperm, warningMessages);
        }
    }
}