//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="LogicBase.cs" company="Microsoft">
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//  OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------- 

namespace HP.ICE.BusinessLogic.Logics
{
    #region Namespaces

    using System.Collections.Generic;

    using HP.ICE.DomainModels.Custom;

    #endregion

    /// <summary>
    /// Class LogicBase.
    /// </summary>
    public class LogicBase
    {
        /// <summary>
        /// Base class for Logic classes
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorMessages">The error messages.</param>
        protected void UpdateResultProperties(
            EntityProcessingResult result,
            bool status,
            string errorMessage,
            IList<string> errorMessages = null)
        {
            result.IsValid = result.IsDeleted = status;
            result.ErrorMessage = errorMessage;

            if (errorMessages != null && errorMessages.Count > 0)
            {
                result.ErrorMessages = errorMessages;
            }
        }
    }
}