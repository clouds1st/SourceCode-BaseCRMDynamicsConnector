//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ValidatorBase.cs" company="Microsoft">
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

    using System.Linq;

    using HP.ICE.BusinessLogic.Models;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.Framework.Extensions;
    using HP.ICE.Framework.Helpers;

    #endregion

    /// <summary>
    /// The validator base
    /// </summary>
    public class ValidatorBase
    {
        /// <summary>
        /// The reference value repository
        /// </summary>
        private readonly IReferenceValuesRepository referenceValueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBase"/> class.
        /// </summary>
        public ValidatorBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBase" /> class.
        /// </summary>
        /// <param name="referenceValueRepository">The reference value repository.</param>
        protected ValidatorBase(IReferenceValuesRepository referenceValueRepository)
        {
            Validator.ArgumentNotNull(referenceValueRepository, nameof(IReferenceValuesRepository));

            this.referenceValueRepository = referenceValueRepository;
        }

        /// <summary>
        /// Constructs the entity validation result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Returns entity validation result
        /// </returns>
        public static EntityValidationResult ConstructEntityValidationResult(string message)
        {
            return new EntityValidationResult { IsValid = string.IsNullOrEmpty(message), ErrorMessage = message };
        }

        /// <summary>
        /// Determines whether [is entity in draft state] [the specified reference value identifier].
        /// </summary>
        /// <param name="referenceValueId">The reference value identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is entity in draft state] [the specified reference value identifier]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsEntityInDraftState(int referenceValueId)
        {
            return
                CommonHelpers.IsEqualsWithIgnoreCase(
                    this.referenceValueRepository.Get(x => x.ReferenceValueId == referenceValueId)
                        .FirstOrDefault()?.ReferenceValue,
                    Constants.DraftStatus);
        }
    }
}