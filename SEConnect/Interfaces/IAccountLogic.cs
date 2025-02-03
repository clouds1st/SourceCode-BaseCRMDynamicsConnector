// -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="IAccountLogic.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.Interfaces
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Linq;

    using HP.ICE.DomainModels;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.DomainModels.ViewModel;

    #endregion

    /// <summary>
    /// The account logic
    /// </summary>
    public interface IAccountLogic
    {
        /// <summary>
        /// Gets the account details.
        /// </summary>
        /// <param name="queryCriteria">The query criteria.</param>
        /// <returns>Returns account details</returns>
        IEnumerable<AutoCompleteSearchResult> GetAccountDetails(AccountQueryCriteria queryCriteria);
    }
}