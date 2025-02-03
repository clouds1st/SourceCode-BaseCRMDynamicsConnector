//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="EmployeeDetailSearchCriteria.cs" company="Microsoft">
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
    /// Class EmployeeDetailSearchCriteria
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EmployeeDetailSearchCriteria
    {
        /// <summary>
        /// Gets or sets the planning period Identifier
        /// </summary>
        /// <value>The planning period identifier.</value>
        public int PlanningPeriodId { get; set; }

        /// <summary>
        /// Gets or sets the Sales Organization Identifier
        /// </summary>
        /// <value>The sales organization identifier.</value>
        public long SalesOrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the without manager
        /// </summary>
        /// <value>The without manager.</value>
        public string WithoutManager { get; set; }
    }
}