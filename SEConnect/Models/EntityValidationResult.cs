//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="EntityValidationResult.cs" company="Microsoft">
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//  OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------- 

namespace HP.ICE.BusinessLogic.Models
{
    /// <summary>
    /// The entity validation result
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public struct EntityValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidationResult" /> struct.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="errorMessage">The error message.</param>
        public EntityValidationResult(bool isValid, string errorMessage)
        {
            this.IsValid = isValid;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether entity is valid or not
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>The error messages.</value>
        public string ErrorMessage { get; set; }
    }
}