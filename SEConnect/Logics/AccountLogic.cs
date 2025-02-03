// -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="AccountLogic.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.Logics
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.DomainModels.ViewModel;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;
    using HP.ICE.Framework.Utilities;

    #endregion

    /// <summary>
    /// The account logic
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IAccountLogic" />
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IAnaplanLogic" />
    public class AccountLogic : IAccountLogic
    {
        /// <summary>
        /// The account repository
        /// </summary>
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountLogic"/> class.
        /// </summary>
        /// <param name="accountRepository">The account repository.</param>
        public AccountLogic(IAccountRepository accountRepository)
        {
            Validator.ArgumentNotNull(accountRepository, nameof(accountRepository));

            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Gets the account details.
        /// </summary>
        /// <param name="accountQueryCriteria">The account query criteria.</param>
        /// <returns>
        /// Returns account details
        /// </returns>
        public IEnumerable<AutoCompleteSearchResult> GetAccountDetails(AccountQueryCriteria accountQueryCriteria)
        {
            return this.accountRepository.GetAccountDetails(accountQueryCriteria);
        }
    }
}