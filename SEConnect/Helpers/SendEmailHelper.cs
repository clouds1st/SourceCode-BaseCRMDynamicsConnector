//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="SendEmailHelper.cs" company="Microsoft">
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//  OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------- 

namespace HP.ICE.BusinessLogic.Helpers
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.Framework.Email;
    using HP.ICE.Framework.Extensions;

    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>
    /// Send Email Notification helper class
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.ISendEmailHelper" />
    public class SendEmailHelper : ISendEmailHelper
    {
        /// <summary>
        /// Email Sender
        /// </summary>
        private readonly IEmailSender emailSender;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SendEmailHelper> logger;

        /// <summary>
        /// Message Type repository
        /// </summary>
        private readonly IMessageTypeTemplateRepository messageTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailHelper" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger</param>
        /// <param name="messageTypeRepository">Message Type repository</param>
        /// <param name="emailSender">The email sender</param>
        public SendEmailHelper(
            ILoggerFactory loggerFactory,
            IMessageTypeTemplateRepository messageTypeRepository,
            IEmailSender emailSender)
        {
            this.logger = loggerFactory.CreateLogger<SendEmailHelper>();
            this.messageTypeRepository = messageTypeRepository;
            this.emailSender = emailSender;
        }

        /// <summary>
        /// Send Email Async Task
        /// </summary>
        /// <param name="emailNotificationRequest"> Email Notification Request </param>
        /// <returns> Task Entity Processing Result </returns>
        public async Task<EntityProcessingResult> SendEmail(EmailNotificationRequest emailNotificationRequest)
        {
            var entityProcessingResult = new EntityProcessingResult
            {
                IsValid = true,
                ErrorMessages = new List<string>()
            };
            Validator.ArgumentNotNull(emailNotificationRequest, nameof(EmailNotificationRequest));

            EmailTemplate emailTemplate;
            if (emailNotificationRequest.Template != null)
            {
                emailTemplate = emailNotificationRequest.Template;
            }
            else
            {
                emailTemplate =
                    this.messageTypeRepository.GetEmailTemplateForMessageType(
                        emailNotificationRequest.MessageTypeId,
                        emailNotificationRequest.RequestStatusId);
                if (emailTemplate == null)
                {
                    var msg = string.Concat(
                        Constants.NoEmailTemplateConfiguredForMessageType,
                        emailNotificationRequest.MessageTypeId);
                    this.logger.LogError(msg);
                    entityProcessingResult.ErrorMessages.Add(msg);
                    entityProcessingResult.IsValid = false;
                    return entityProcessingResult;
                }
            }

            string formattedEmailBody;

            if (emailNotificationRequest.SubstitutionParams != null)
            {
                formattedEmailBody = EmailMessageFormatter.GetFormattedEmailBody(
                    emailTemplate.Body,
                    emailNotificationRequest.SubstitutionParams);
            }
            else
            {
                formattedEmailBody = emailTemplate.Body;
            }

            try
            {
                await this.emailSender.SendMail(
                    formattedEmailBody,
                    emailNotificationRequest.UserId,
                    emailTemplate.Subject,
                    emailNotificationRequest.CCUserIds,
                    emailNotificationRequest.AttentionInd,
                    attachments: emailNotificationRequest.MailAttachments);

                this.logger.LogInformation(
                    $"Successfully sent email notification to user {emailNotificationRequest.UserId} for message type {emailNotificationRequest.MessageTypeId}");
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException ex)
            {
                entityProcessingResult.IsValid = true;
                var failedRecipient = string.Empty;

                foreach (var item in ex.InnerExceptions)
                {
                    failedRecipient += item.FailedRecipient + ",";
                }

                failedRecipient = failedRecipient.Remove(failedRecipient.Length - 1);
                entityProcessingResult.ErrorMessages.Add("Mailbox Unavailable for user " + failedRecipient);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException ex)
            {
                entityProcessingResult.IsValid = true;
                entityProcessingResult.ErrorMessages.Add("Mailbox Unavailable for user " + ex.FailedRecipient);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                entityProcessingResult.IsValid = false;
                entityProcessingResult.ErrorMessages.Add("Unable to send email to users becasue " + ex.Message);
            }
            catch (System.Exception ex)
            {
                entityProcessingResult.IsValid = false;
                entityProcessingResult.ErrorMessages.Add("Unable to send email to users becasue " + ex.Message);
            }

            return entityProcessingResult;
        }
    }
}