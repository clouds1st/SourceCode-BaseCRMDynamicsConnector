//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="CreditControlRuleValidator.cs" company="Microsoft">
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
    using System;
    using DomainModels;
    using DomainModels.Custom;
    using Framework.Extensions;
    using Interfaces;
    using Models;

    /// <summary>
    /// validator for credit control entity
    /// </summary>
    public class CreditControlRuleValidator : ICreditControlRuleValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditControlRuleValidator" /> class.
        /// </summary>
        public CreditControlRuleValidator()
        {
        }

        /// <summary>
        /// validates credit control entity
        /// </summary>
        /// <param name="creditControlRule">entity to validate</param>
        /// <returns>returns entity processing result</returns>
        public EntityProcessingResult ValidateEntity(CreditControlRule creditControlRule)
        {
            Validator.ArgumentNotNull(creditControlRule, nameof(creditControlRule));
            var entityProcessingResult = new EntityProcessingResult();
            if (string.IsNullOrWhiteSpace(creditControlRule.CreditControlRuleName) || creditControlRule.CreditControlRuleName.Length > 1000)
            {
                entityProcessingResult.ErrorMessage = Constants.InvalidCreditControlRuleName;
                entityProcessingResult.IsValid = false;
            }
            else if (creditControlRule.SalesOrganizationId <= 0)
            {
                entityProcessingResult.ErrorMessage = Constants.InvalidCreditControlSalesOrganization;
                entityProcessingResult.IsValid = false;
            }
            else if (creditControlRule.IsAllSalesPersonIncluded == false && (creditControlRule.CreditControlRuleSalesPerson == null || creditControlRule.CreditControlRuleSalesPerson.Count == 0))
            {
                entityProcessingResult.ErrorMessage = Constants.InvalidCreditControlSalesPerson;
                entityProcessingResult.IsValid = false;
            }
            else if (creditControlRule.CreditControlRuleValidFrom > creditControlRule.CreditControlRuleValidTo)
            {
                entityProcessingResult.ErrorMessage = Constants.InvalidCreditControlFromToDateComparison;
                entityProcessingResult.IsValid = false;
            }

            return entityProcessingResult;
        }
    }
}
