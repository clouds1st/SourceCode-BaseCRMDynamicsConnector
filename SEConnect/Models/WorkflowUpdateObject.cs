// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowUpdateObject.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SEConnect.Business.Models
{
    #region Namespaces

    using System;

    #endregion

    /// <summary>
    /// The workflow update object
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class WorkflowUpdateObject
    {
        /// <summary>
        /// Gets or sets the search end date.
        /// </summary>
        /// <value>
        /// The search end date.
        /// </value>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>
        /// The selected Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the object type identifier.
        /// </summary>
        /// <value>
        /// The object type identifier.
        /// </value>
        public int ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the search start date.
        /// </summary>
        /// <value>
        /// The search start date.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The name of the status
        /// </value>
        public int? Status { get; set; }
    }
}