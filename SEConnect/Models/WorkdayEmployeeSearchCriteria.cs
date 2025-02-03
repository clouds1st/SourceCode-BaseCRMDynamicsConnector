//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="WorkdayEmployeeSearchCriteria.cs" company="Microsoft">
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
    #region Namespaces

    using System;

    #endregion

    /// <summary>
    /// The WorkdayEmployeeSearchCriteria Class
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class WorkdayEmployeeSearchCriteria
    {
        /// <summary>
        /// Gets or sets the Business Group
        /// </summary>
        public string BusinessGroup { get; set; }

        /// <summary>
        /// Gets or sets the Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Employee Identifier
        /// </summary>
        /// <value>The employee identifier.</value>2
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Employee First Name
        /// </summary>      
        /// <value>The first name of the employee.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the From Date
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Gets or sets the Employee Last Name
        /// </summary>
        /// <value>The last name of the employee.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Manager Identifier
        /// </summary>
        /// <value>The Manager identifier.</value>
        public string ManagerId { get; set; }

        /// <summary>
        /// Gets or sets the Manager Name
        /// </summary>
        /// <value>The Manager Name.</value>
        public string ManagerName { get; set; }

        /// <summary>
        /// Gets or sets the Planning Period Identifier
        /// </summary>
        /// <value>The planning period identifier.</value>
        public int PlanningPeriodId { get; set; }

        /// <summary>
        /// Gets or sets the Sub Company Code
        /// </summary>
        /// <value>The Sub Company Code.</value>
        public string SubCompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the To Date
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}