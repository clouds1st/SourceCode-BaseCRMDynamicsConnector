// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeSearchResult.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.Models
{
    #region Namespaces

    using System;
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// The EmployeeSearchResult Class
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EmployeeSearchResult
    {
        [Key]

        /// <summary>
        /// Gets or sets the Employee Identifier
        /// </summary>
        /// <value>The employee identifier.</value>
        public long EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Employee Name
        /// </summary>
        /// <value>The name of the employee.</value>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Gets or sets the Member Identifier
        /// </summary>
        /// <value>The member identifier.</value>
        public long MemberId { get; set; }

        /// <summary>
        /// Gets or sets the Employee Number
        /// </summary>
        /// <value>The employee number.</value>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the End Date
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the HR Manager Identifier
        /// </summary>
        /// <value>The hr manager identifier.</value>
        public long HRManagerId { get; set; }

        /// <summary>
        /// Gets or sets the HR Manager Name
        /// </summary>
        /// <value>The name of the hr manager.</value>
        public string HRManagerName { get; set; }

        /// <summary>
        /// Gets or sets the Sales Organization Identifier
        /// </summary>
        /// <value>The sales organization identifier.</value>
        public long SalesOrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the Sales Organization Number
        /// </summary>
        /// <value>The sales organization number.</value>
        public string SalesOrganizationNumber { get; set; }

        /// <summary>
        /// Gets or sets the Sales Organization Name
        /// </summary>
        /// <value>The name of the sales organization.</value>
        public string SalesOrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the Start Date
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the Supervisor Flag
        /// </summary>
        /// <value>The supervisor flag.</value>
        public string SupervisorFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether employee is Supervisor
        /// </summary>
        /// <value><c>true</c> if this instance is supervisor; otherwise, <c>false</c>.</value>
        public bool IsSupervisor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [GHRMS pending update ind].
        /// </summary>
        /// <value>
        /// <c>true</c> if [GHRMS pending update ind]; otherwise, <c>false</c>.
        /// </value>
        public bool GHRMSPendingUpdateInd { get; set; }

        /// <summary>
        /// Gets or sets the GHRMS pending update description.
        /// </summary>
        /// <value>
        /// The GHRMS pending update description.
        /// </value>
        public string GHRMSPendingUpdateDescription { get; set; }

        /// <summary>
        /// Gets or sets the sales person resource type code.
        /// </summary>
        /// <value>
        /// The sales person resource type code.
        /// </value>
        public int? SalesPersonResourceTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the sales person resource status code.
        /// </summary>
        /// <value>
        /// The sales person resource status code.
        /// </value>
        public int? SalesPersonResourceStatusCode { get; set; }
    }
}