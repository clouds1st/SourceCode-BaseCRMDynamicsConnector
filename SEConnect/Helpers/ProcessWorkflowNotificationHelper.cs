// -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ProcessWorkflowNotificationHelper.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.Helpers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.DomainModels.ViewModel;
    using HP.ICE.Framework;
    using HP.ICE.Framework.Configuration;
    using HP.ICE.Framework.Constants;
    using HP.ICE.Framework.Email;
    using HP.ICE.Framework.ServiceBus;

    using Microsoft.Extensions.Logging;
    #endregion

    /// <summary>
    /// Process Workflow Notification helper class
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IProcessWorkflowNotificationHelper" />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ProcessWorkflowNotificationHelper : IProcessWorkflowNotificationHelper
    {
        /// <summary>
        /// The reference value repository
        /// </summary>
        private readonly IReferenceValueCategoryRepository referenceValueCategoryRepository;

        /// <summary>
        /// WorkFlow Setup Repository
        /// </summary>
        private readonly IWorkflowSetupRepository workflowSetupRepository;

        /// <summary>
        /// Employee Repository
        /// </summary>
        private readonly IEmployeeRepository employeeRepository;

        /// <summary>
        /// Sales Letter Version Repository
        /// </summary>
        private readonly ISalesLetterVersionRepository salesLetterVersionRepository;

        /// <summary>
        /// Sales Letter Version Notification Email Repository
        /// </summary>
        private readonly ISalesLetterVersionNotificationEmailRepository salesLetterVersionNotificationEmailRepository;

        /// <summary>
        /// The email sender
        /// </summary>
        private readonly IEmailSender emailSender;

        /// <summary>
        /// object workflow status
        /// </summary>
        private readonly IObjectWorkflowStatus objectWorkflowStatusLogic;

        /// <summary>
        /// Sales Person Target Plan Repository
        /// </summary>
        private readonly ISalesPersonTargetPlanRepository setpRepository;

        /// <summary>
        /// User Resolver Service
        /// </summary>
        private readonly IUserResolverService userResolverService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessWorkflowNotificationHelper" /> class.
        /// </summary>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="referenceValueCategoryRepository">Reference Value Category Repository</param>
        /// <param name="workflowSetupRepository">WorkFlow Setup Repository</param>
        /// <param name="employeeRepository">Employee Repository</param>
        /// <param name="salesLetterVersionRepository">sales letter version repository</param>
        /// <param name="salesLetterVersionNotificationEmailRepository">Notification Email Repository</param>
        /// <param name="emailSender"> The email sender</param>
        /// <param name="objectWorkflowStatusLogic"> The Object Flow Logic</param>
        /// <param name="setpRepository">Sales person target plan repository</param>
        /// <param name="userResolverService">User Resolver Service</param>
        public ProcessWorkflowNotificationHelper(
            ConfigurationOptions configurationOptions,
            IReferenceValueCategoryRepository referenceValueCategoryRepository,
            IWorkflowSetupRepository workflowSetupRepository,
            IEmployeeRepository employeeRepository,
            ISalesLetterVersionRepository salesLetterVersionRepository,
            ISalesLetterVersionNotificationEmailRepository salesLetterVersionNotificationEmailRepository,
            IEmailSender emailSender,
            IObjectWorkflowStatus objectWorkflowStatusLogic,
            ISalesPersonTargetPlanRepository setpRepository,
            IUserResolverService userResolverService)
        {
            this.ConfigurationOptions = configurationOptions;
            this.referenceValueCategoryRepository = referenceValueCategoryRepository;
            this.workflowSetupRepository = workflowSetupRepository;
            this.employeeRepository = employeeRepository;
            this.salesLetterVersionRepository = salesLetterVersionRepository;
            this.salesLetterVersionNotificationEmailRepository = salesLetterVersionNotificationEmailRepository;
            this.emailSender = emailSender;
            this.objectWorkflowStatusLogic = objectWorkflowStatusLogic;
            this.setpRepository = setpRepository;
            this.userResolverService = userResolverService;
        }

        /// <summary>
        /// Gets the configuration options.
        /// </summary>
        /// <value>The configuration options.</value>
        private ConfigurationOptions ConfigurationOptions { get; }

        /// <summary>
        /// send notification
        /// </summary>
        /// <param name="notificationObject">workflow notification object</param>
        /// <returns>asynchronous task</returns>
        public async Task SendNotification(WorkflowNotification notificationObject)
        {
            var serviceBusConnector = new ServiceBusConnector(this.ConfigurationOptions, new LoggerFactory());
            await serviceBusConnector.SendMessageToServiceBusQueue(
                QueueConstants.WorkflowTasksNotificationQueue,
                notificationObject);
        }

        /// <summary>
        /// Process Sales Letter Status and Send Email
        /// </summary>
        /// <param name="salesLetterManagementUpdateList">List of Sales Letter Update Object</param>
        /// <param name="categoryType">Type of the category.</param>
        /// <param name="statusType">Type of the status.</param>
        /// <returns>Entity Processing Result</returns>
        public EntityProcessingResult SalesLetterStatusProcessing(
            IEnumerable<SalesLetterUpdateStatusViewModel> salesLetterManagementUpdateList,
            string categoryType,
            SalesLetterStatus statusType)
        {
            var entityProcessingResult = new EntityProcessingResult() { ErrorMessages = new List<string>() };
            var failedLetter = new List<SalesLetterUpdateStatusViewModel>();

            // all the request would have same validation criteria and and target status
            var referenceValuesForCategory =
                this.referenceValueCategoryRepository.Get(
                    x => x.CategoryName.Equals(categoryType),
                    null,
                    x => x.ReferenceValues).FirstOrDefault();
            var moduleType = salesLetterManagementUpdateList.FirstOrDefault()?.ModuleType;
            var targetStatusTypeCode = salesLetterManagementUpdateList.FirstOrDefault()?.TargetStatusTypeCode;
            var targetStatusTypeId =
                referenceValuesForCategory.ReferenceValues.Where(x => x.ReferenceValue.Equals(targetStatusTypeCode))
                    .FirstOrDefault()
                    .ReferenceValueId;
            foreach (var salesLetterUpdate in salesLetterManagementUpdateList)
            {
                var isValidRequest = false;
                var errorMessage = string.Empty;
                var currentSalesLetterVersion =
                    this.salesLetterVersionRepository.FindBy(
                            x =>
                                x.IsDeleted != true && x.SalesLetterId.Equals(salesLetterUpdate.SalesLetterId)
                                && x.SalesLetterVersionNumber.Equals(salesLetterUpdate.SalesLetterVersionNumber))
                        .FirstOrDefault();
                if (currentSalesLetterVersion != null)
                {
                    if (
                        currentSalesLetterVersion.SalesLetterVersionStatusCode.Equals(
                            salesLetterUpdate.CurrentStatusTypeId))
                    {
                        isValidRequest = true;
                    }

                    if (isValidRequest)
                    {
                        var dateTime = DateTime.Now;
                        if (!this.GetWorkFlowDetailsAndSendEmail(salesLetterUpdate, entityProcessingResult, targetStatusTypeId, currentSalesLetterVersion))
                        {
                            failedLetter.Add(salesLetterUpdate);
                            continue;
                        }

                        if (WorkFlowObjectTypes.SetpWorkFlow == moduleType)
                        {
                            this.UpdateStatus(
                                currentSalesLetterVersion,
                                targetStatusTypeId,
                                statusType,
                                salesLetterUpdate.EmployeeId,
                                dateTime);

                            referenceValuesForCategory =
                                this.referenceValueCategoryRepository.Get(
                                    x => x.CategoryName.Equals(Constants.CategoryNameForSETP),
                                    null,
                                    x => x.ReferenceValues).FirstOrDefault();
                            targetStatusTypeCode =
                                salesLetterManagementUpdateList.FirstOrDefault()?.TargetStatusTypeCode;
                            var salesLetterStatusCode =
                                referenceValuesForCategory.ReferenceValues.Where(
                                        x => x.ReferenceValue.Equals(targetStatusTypeCode))
                                    .FirstOrDefault()
                                    .ReferenceValueId;

                            var currentSETP =
                                this.setpRepository.FindBy(
                                    x =>
                                        x.IsDeleted != true
                                        && x.SalesPersonTargetPlanId.Equals(
                                            currentSalesLetterVersion.SalesPersonTargetPlanId)).FirstOrDefault();
                            this.UpdateSetpStatus(currentSETP, salesLetterStatusCode);
                        }
                        else
                        {
                            this.UpdateStatus(
                                currentSalesLetterVersion,
                                targetStatusTypeId,
                                statusType,
                                salesLetterUpdate.EmployeeId,
                                dateTime);
                            if (statusType != SalesLetterStatus.EscalationUpdate)
                            {
                                referenceValuesForCategory =
                                    this.referenceValueCategoryRepository.Get(
                                        x => x.CategoryName.Equals(Constants.CategoryNameForSETP),
                                        null,
                                        x => x.ReferenceValues).FirstOrDefault();
                                targetStatusTypeCode =
                                    salesLetterManagementUpdateList.FirstOrDefault()?.TargetStatusTypeCode;
                                var salesLetterStatusCode =
                                    referenceValuesForCategory.ReferenceValues.Where(
                                            x => x.ReferenceValue.Equals(targetStatusTypeCode))
                                        .FirstOrDefault()
                                        .ReferenceValueId;
                                var currentSETP =
                                    this.setpRepository.FindBy(
                                        x =>
                                            x.IsDeleted != true
                                            && x.SalesPersonTargetPlanId.Equals(
                                                currentSalesLetterVersion.SalesPersonTargetPlanId)).FirstOrDefault();
                                this.UpdateSetpStatus(currentSETP, salesLetterStatusCode);
                            }
                        }
                    }
                    else
                    {
                        // ToDO Remove Hard Coded Message
                        entityProcessingResult.ErrorMessages.Add(
                            $"Sales Letter {salesLetterUpdate.SalesLetterId} Status has changed since it was requested");
                    }
                }
                else
                {
                    // TODO Remove Hard Coded Message
                    entityProcessingResult.ErrorMessages.Add(
                        $"Sales {salesLetterUpdate.SalesLetterId} Letter Not Found");
                }
            }

            if (failedLetter.Any())
            {
                if (!entityProcessingResult.ErrorMessages.Any())
                {
                    var salesOrgIds = string.Join(",", failedLetter.Select(x => x.SalesOrgId));
                    var salesLetterIds = string.Join(",", failedLetter.Select(x => x.SalesLetterId));
                    entityProcessingResult.ErrorMessages.Add(
                        $"Workflow is not configured for Sales Orgs: {salesOrgIds}, Status not changed for sales letters: {salesLetterIds}");
                }
            }

            return entityProcessingResult;
        }

        /// <summary>
        /// Sales letter status notification.
        /// </summary>
        /// <param name="salesLetterManagementUpdateList">The sales letter management update list.</param>
        /// <param name="categoryType">Type of the category.</param>
        /// <param name="statusType">Type of the status.</param>
        /// <param name="toRecepients">To recipients.</param>
        /// <param name="ccRecepients">The cc recipients.</param>
        /// <returns>
        /// Entity Processing Result
        /// </returns>
        public EntityProcessingResult SalesLetterStatusNotification(
            IEnumerable<SalesLetterUpdateStatusViewModel> salesLetterManagementUpdateList,
            string categoryType,
            SalesLetterStatus statusType,
            string toRecepients,
            string ccRecepients)
        {
            var entityProcessingResult = new EntityProcessingResult() { ErrorMessages = new List<string>() };
            var failedLetter = new List<SalesLetterUpdateStatusViewModel>();

            // all the request would have same validation criteria and and target status
            var referenceValuesForCategory =
                this.referenceValueCategoryRepository.Get(
                    x => x.CategoryName.Equals(categoryType),
                    null,
                    x => x.ReferenceValues).FirstOrDefault();
            var moduleType = salesLetterManagementUpdateList.FirstOrDefault()?.ModuleType;
            var targetStatusTypeCode = salesLetterManagementUpdateList.FirstOrDefault()?.TargetStatusTypeCode;
            var targetStatusTypeId =
                referenceValuesForCategory.ReferenceValues.Where(x => x.ReferenceName.Equals(targetStatusTypeCode))
                    .FirstOrDefault()
                    .ReferenceValueId;
            foreach (var salesLetterUpdate in salesLetterManagementUpdateList)
            {
                var currentSalesLetterVersion =
                    this.salesLetterVersionRepository.FindBy(
                            x =>
                                x.IsDeleted != true && x.SalesLetterId.Equals(salesLetterUpdate.SalesLetterId)
                                && x.SalesLetterVersionNumber.Equals(salesLetterUpdate.SalesLetterVersionNumber))
                        .FirstOrDefault();
                if (currentSalesLetterVersion != null)
                {
                    var dateTime = DateTime.Now;
                    if (
                        !this.GetWorkFlowDetailsAndSendEmailNotification(
                            salesLetterUpdate,
                            entityProcessingResult,
                            dateTime,
                            toRecepients,
                            ccRecepients,
                            targetStatusTypeId))
                    {
                        failedLetter.Add(salesLetterUpdate);
                        continue;
                    }

                    this.UpdateStatus(
                        currentSalesLetterVersion,
                        targetStatusTypeId,
                        statusType,
                        salesLetterUpdate.EmployeeId,
                        dateTime);
                }
                else
                {
                    // TODO Remove Hard Coded Message
                    entityProcessingResult.ErrorMessages.Add(
                        $"Sales {salesLetterUpdate.SalesLetterId} Letter Not Found");
                }
            }

            return entityProcessingResult;
        }

        /// <summary>
        /// Get Workflow details for selected sales letter
        /// </summary>
        /// <param name="salesLetteUpdate">Selected Sales Letter Details</param>
        /// <param name="entityProcessingResult">entity processing result</param>
        /// <param name="targetStatusTypeId">target Status Type Id</param>
        /// <param name="salesLetterVersion">The sales letter version.</param>
        /// <returns>
        /// True if email sent else false
        /// </returns>
        public bool GetWorkFlowDetailsAndSendEmail(SalesLetterUpdateStatusViewModel salesLetteUpdate, EntityProcessingResult entityProcessingResult, int targetStatusTypeId, SalesLetterVersion salesLetterVersion)
        {
            var ccAddress = string.Empty;
            var workflowNotificationObject = this.GetWorkflowNotification(salesLetteUpdate, targetStatusTypeId);
            var referenceValuecategory =
                this.referenceValueCategoryRepository.Get(
                    x => x.CategoryName.Equals(Constants.CategoryNameForObjectType),
                    null,
                    x => x.ReferenceValues).FirstOrDefault();
            var firstOrDefault =
                referenceValuecategory?.ReferenceValues?.FirstOrDefault(
                    x => x.ReferenceValue.Equals(workflowNotificationObject.WorkFlowObjectTypeName));
            var toAddress =
                this.employeeRepository.FindBy(x => x.EmployeeId.Equals(salesLetteUpdate.EmployeeId))
                    .FirstOrDefault()?.Email;
            if (firstOrDefault != null)
            {
                var referenceValueId = firstOrDefault.ReferenceValueId;
                workflowNotificationObject.WorkFlowObjectTypeId = referenceValueId;
            }

            this.objectWorkflowStatusLogic.SalesLetterUpdate = salesLetteUpdate;
            ////Getting Manager user Email and Sco Email
            var actionCode = salesLetteUpdate.TargetStatusTypeCode;
            var isProductionEnvironment = ConfigurationOptions.Email.SendToDefault;
            if (isProductionEnvironment)
            {
                if (actionCode == Constants.RELEASED || actionCode == Constants.ESCALATIONUPDATE)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.EmployeeEmailId;

                    ccAddress = !string.IsNullOrEmpty(salesLetteUpdate.ManagerEmailId) ? salesLetteUpdate.ManagerEmailId + "," + this.userResolverService.GetLoggedInUserEmail() : this.userResolverService.GetLoggedInUserEmail();

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? ccAddress + "," + scoEmail : ccAddress;
                }
                else if (actionCode == Constants.REWORK)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = !string.IsNullOrEmpty(scoEmail) ? scoEmail : this.userResolverService.GetLoggedInUserEmail();

                    ccAddress = !string.IsNullOrEmpty(salesLetteUpdate.ManagerEmailId) ? salesLetteUpdate.ManagerEmailId + "," + this.userResolverService.GetLoggedInUserEmail() : this.userResolverService.GetLoggedInUserEmail();
                }
                else if (actionCode == Constants.REWORKREQUESTED)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.ManagerEmailId;

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? scoEmail + "," + this.userResolverService.GetLoggedInUserEmail() : this.userResolverService.GetLoggedInUserEmail();
                }
                else if (actionCode == Constants.ESCALATED)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.ManagerEmailId;

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? scoEmail + "," + this.userResolverService.GetLoggedInUserEmail() : this.userResolverService.GetLoggedInUserEmail();
                }
                else if (actionCode == Constants.ACCEPTED)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.ManagerEmailId;

                    ccAddress = salesLetteUpdate.EmployeeEmailId + "," + this.userResolverService.GetLoggedInUserEmail();

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? ccAddress + "," + scoEmail : ccAddress;
                }
                else if (actionCode == Constants.NOTIFIED)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.ManagerEmailId;

                    ccAddress = this.userResolverService.GetLoggedInUserEmail();

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? ccAddress + "," + scoEmail : ccAddress;
                }
                else if (actionCode == Constants.UpdateSelectedToAuditPassedAssChanges)
                {
                    string scoEmail = this.GetEmailFromSalesLetterVersion(salesLetterVersion);

                    toAddress = salesLetteUpdate.EmployeeEmailId;

                    ccAddress = salesLetteUpdate.ManagerEmailId + "," + this.userResolverService.GetLoggedInUserEmail();

                    ccAddress = !string.IsNullOrEmpty(scoEmail) ? ccAddress + "," + scoEmail : ccAddress;
                }
            }
            else
            {
                toAddress = this.ConfigurationOptions.Email.DefaultToAddress;
                ccAddress = this.ConfigurationOptions.Email.DefaultCCAddress + "," + this.userResolverService.GetLoggedInUserEmail();
            }

            // (false) is for not to trigger actual workflow only to send sync email 
            var res =
                this.objectWorkflowStatusLogic.ProcessWorkflowNotificationRequest(
                    toAddress,
                    ccAddress,
                    workflowNotificationObject,
                    false).Result;
            //// if email send has error/ warning
            foreach (var errorMessage in res.ErrorMessages)
            {
                entityProcessingResult.ErrorMessages.Add(errorMessage);
            }

            if (!res.IsValid)
            {
                entityProcessingResult.IsValid = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Email Id from Sales Letter Version
        /// </summary>
        /// <param name="salesLetterVersion">sales Letter Version</param>
        /// <returns>returns string </returns>
        private string GetEmailFromSalesLetterVersion(SalesLetterVersion salesLetterVersion)
        {
            var createdBy = salesLetterVersion.CreatedBy.ToLower();
            string emailId = string.Empty;
            long empNumber = 0;
            if (createdBy != "system")
            {
                if (createdBy.IndexOf("@") != -1)
                {
                    return createdBy;
                }

                if (long.TryParse(createdBy, out empNumber))
                {
                    return this.employeeRepository.FindBy(x => x.EmployeeNumber.Equals(createdBy)).FirstOrDefault()?.Email;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get Workflow details for selected sales letter
        /// </summary>
        /// <param name="salesLetteUpdate">Selected Sales Letter Details</param>
        /// <param name="entityProcessingResult">The entity processing result.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="toRecepients">To recipients.</param>
        /// <param name="ccRecepients">The cc recipients.</param>
        /// <param name="targetStatusTypeId">The target status type identifier.</param>
        /// <returns>
        /// True if email sent else false
        /// </returns>
        private bool GetWorkFlowDetailsAndSendEmailNotification(
            SalesLetterUpdateStatusViewModel salesLetteUpdate,
            EntityProcessingResult entityProcessingResult,
            DateTime dateTime,
            string toRecepients,
            string ccRecepients,
            int targetStatusTypeId)
        {
            var ccAddress = ccRecepients;

            var workflowNotificationObject = this.GetWorkflowNotification(salesLetteUpdate, targetStatusTypeId);
            var referenceValuecategory =
                this.referenceValueCategoryRepository.Get(
                    x => x.CategoryName.Equals(Constants.CategoryNameForObjectType),
                    null,
                    x => x.ReferenceValues).FirstOrDefault();
            var firstOrDefault =
                referenceValuecategory?.ReferenceValues?.FirstOrDefault(
                    x => x.ReferenceValue.Equals(workflowNotificationObject.WorkFlowObjectTypeName));
            var toAddress = toRecepients;

            // this.employeeRepository.FindBy(x => x.EmployeeId.Equals(salesLetteUpdate.EmployeeId)).FirstOrDefault()?.Email;

            // ccRecepients = String.Join(COMMA, workFlowSetup.ApprovalLevel1, currentEmpLetter.SCOEmail);
            if (firstOrDefault != null)
            {
                var referenceValueId = firstOrDefault.ReferenceValueId;
                workflowNotificationObject.WorkFlowObjectTypeId = referenceValueId;
            }

            var workFlowItems = this.workflowSetupRepository.GetWorkflowItems(workflowNotificationObject);
            if (!workFlowItems.Any())
            {
                entityProcessingResult.ErrorMessages.Add(
                    "Work flow is not setup for sales letter notified status of selected sales organization");
            }
            else
            {
                foreach (var workflowSetup in workFlowItems)
                {
                    if (
                        workflowSetup.WorkflowSetupSalesOrg.All(
                            x => !x.SalesOrganizationId.Equals(salesLetteUpdate.SalesOrgId)))
                    {
                        continue;
                    }

                    if (workflowSetup.EmailRequiredInd)
                    {
                        if (this.ConfigurationOptions.Email.SendToDefault)
                        {
                            this.emailSender.SendMail(
                                workflowSetup.EmailBody,
                                this.ConfigurationOptions.Email.DefaultToAddress,
                                workflowSetup.EmailSubject,
                                this.ConfigurationOptions.Email.DefaultCCAddress).Wait();
                        }
                        else
                        {
                            this.emailSender.SendMail(
                                workflowSetup.EmailBody,
                                toAddress,
                                workflowSetup.EmailSubject,
                                ccAddress).Wait();
                        }

                        var emailNotification = new SalesLetterVersionNotificationEmail
                        {
                            SalesLetterVersionId = salesLetteUpdate.SalesLetterVersionId,
                            SalesLetterId = salesLetteUpdate.SalesLetterId,
                            WorkflowSetupId = workflowSetup.WorkflowSetupId,
                            NotificationTimeStamp = dateTime,
                            StatusCode = null,
                            RecipientList = toAddress,
                            Ccrecipient = workflowSetup.Ccemail,
                            SubjectText = workflowSetup.EmailSubject,
                            BodyText = workflowSetup.EmailBody,
                            IsDeleted = false,
                            ObjectState = ObjectState.Added,
                        };
                        this.salesLetterVersionNotificationEmailRepository.AddEntityIncludingChildren(emailNotification);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Remove Char
        /// </summary>
        /// <param name="ccemail">The email.</param>
        /// <returns>
        /// returns string
        /// </returns>
        private string RemoveSpecialChar(string ccemail)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(ccemail))
            {
                var splitChar = new[] { ',', ';', '!', ' ', '\'' };
                var splitArr = ccemail.Split(splitChar);
                result = string.Join(",", splitArr.Where(s => !string.IsNullOrEmpty(s)));
            }

            return result;
        }

        /// <summary>
        /// Get Workflow Notification object
        /// </summary>
        /// <param name="salesLetteUpdateStatusViewModel">Sales Letter Object</param>
        /// <param name="targetStatusTypeId">Sales Letter target StatusType Id</param>
        /// <returns>
        /// workflow Notification object
        /// </returns>
        private WorkflowNotification GetWorkflowNotification(
            SalesLetterUpdateStatusViewModel salesLetteUpdateStatusViewModel,
            int targetStatusTypeId)
        {
            return new WorkflowNotification()
                       {
                           PlanningPeriodId = salesLetteUpdateStatusViewModel.PlanningPeriodId,
                           SalesOrganizationIds =
                               new[] { salesLetteUpdateStatusViewModel.SalesOrgId },
                           EffectiveDate = DateTime.Now,
                           ObjectPreviousStatusId =
                               salesLetteUpdateStatusViewModel.CurrentStatusTypeId,
                           ObjectCurrentStatusId = targetStatusTypeId,
                           WorkFlowObjectTypeName =
                               salesLetteUpdateStatusViewModel.ModuleType
                               == WorkFlowObjectTypes.SetpWorkFlow
                                   ? WorkFlowObjectTypes.SalesLetterManagementWorkflow
                                   : salesLetteUpdateStatusViewModel.ModuleType,
                           SubstitutionParams =
                               this.GetSubstituteParameters(salesLetteUpdateStatusViewModel)
                       };
        }

        /// <summary>
        /// Update Status of current Sales Letter Version
        /// </summary>
        /// <param name="salesLetterVersion">Current Sales Letter Version</param>
        /// <param name="targetStatus">Target Status Code (Reference Value)</param>
        /// <param name="flowType">Release or Notified</param>
        /// <param name="activeManagerEmployeeId">HR/Sales Manager EmployeeID</param>
        /// <param name="dateTime">The date time.</param>
        private void UpdateStatus(
            SalesLetterVersion salesLetterVersion,
            int targetStatus,
            SalesLetterStatus flowType,
            long activeManagerEmployeeId,
            DateTime dateTime)
        {
            salesLetterVersion.SalesLetterVersionStatusCode = targetStatus;
            switch (flowType)
            {
                case SalesLetterStatus.Notified:
                    salesLetterVersion.ActiveManagerReleaseNotificationTimestamp = dateTime;
                    salesLetterVersion.ActiveManagerEmployeeId = activeManagerEmployeeId;
                    ////salesLetterVersion.ActiveApprovalNotificationInd = true;
                    break;
                case SalesLetterStatus.Released:
                    salesLetterVersion.SalesLetterVersionReleaseTimestamp = dateTime;
                    salesLetterVersion.ReleaseInd = true;
                    break;
            }

            this.salesLetterVersionRepository.Update(salesLetterVersion);
        }

        /// <summary>
        /// Update Status of current Sales Letter Version
        /// </summary>
        /// <param name="salesPersonTargetPlan">Current Sales Letter Version</param>
        /// <param name="targetStatus">Target Status Code (Reference Value)</param>
        private void UpdateSetpStatus(SalesPersonTargetPlan salesPersonTargetPlan, int targetStatus)
        {
            salesPersonTargetPlan.StatusCode = targetStatus;
            this.setpRepository.Update(salesPersonTargetPlan);
        }

        /// <summary>
        /// Gets the substitute parameters.
        /// </summary>
        /// <param name="salesLetteUpdateStatusViewModel">The sales letter update status view model.</param>
        /// <returns>
        /// returns Dictionary
        /// </returns>
        private Dictionary<string, string> GetSubstituteParameters(SalesLetterUpdateStatusViewModel salesLetteUpdateStatusViewModel)
        {
            var subParams = new Dictionary<string, string>
                                {
                                    {
                                        Constants.DeploymentPeriod,
                                        salesLetteUpdateStatusViewModel.DeploymentPeriod
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.FiscalYear,
                                        salesLetteUpdateStatusViewModel.FiscalYear
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.ManagerName,
                                        salesLetteUpdateStatusViewModel.ManagerName
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.SalesOrg,
                                        salesLetteUpdateStatusViewModel.SalesOrgId.ToString()
                                    },
                                    {
                                        Constants.SalesRepID,
                                        salesLetteUpdateStatusViewModel.SalesRepId
                                    },
                                    {
                                        Constants.SalesRepName,
                                        salesLetteUpdateStatusViewModel.SalesRepName
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.SalesRepNames,
                                        salesLetteUpdateStatusViewModel.SalesRepName
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.EmployeeEmailID,
                                        salesLetteUpdateStatusViewModel.EmployeeEmailId
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.ManagerEmailID,
                                        salesLetteUpdateStatusViewModel.ManagerEmailId
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.DeploymentManager,
                                        salesLetteUpdateStatusViewModel.DeploymentManager
                                        ?? string.Empty
                                    },
                                    {
                                        Constants.SCOAdminName,
                                        this.userResolverService.GetSimulatedUserEmail()
                                    },
                                    {
                                        Constants.CommentText,
                                        salesLetteUpdateStatusViewModel.CommentText
                                    },
                                    {
                                        Constants.MgrUpdateText,
                                        salesLetteUpdateStatusViewModel.MgrUpdateText
                                    }
                                };
            return subParams;
        }
    }
}