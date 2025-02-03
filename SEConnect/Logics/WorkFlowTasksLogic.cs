//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="WorkFlowTasksLogic.cs" company="Microsoft">
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
    using System.Linq;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.Framework;
    using HP.ICE.Framework.Extensions;

    #endregion

    /// <summary>
    /// Logic class for Workflow Tasks
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IWorkFlowTasksLogic" />
    public class WorkFlowTasksLogic : IWorkFlowTasksLogic
    {
        /// <summary>
        /// The employee Repository
        /// </summary>
        private readonly IEmployeeRepository employeeRepository;

        /// <summary>
        /// User resolver service
        /// </summary>
        private readonly IUserResolverService userResolverService;

        /// <summary>
        /// Workflow task Repository
        /// </summary>
        private readonly IWorkflowTaskRepository workflowTaskRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkFlowTasksLogic" /> class.
        /// </summary>
        /// <param name="workflowTaskRepository">workflow Task repository</param>
        /// <param name="userResolverService">User resolver service</param>
        /// <param name="employeeRepository">Application user repository</param>
        public WorkFlowTasksLogic(
            IWorkflowTaskRepository workflowTaskRepository,
            IUserResolverService userResolverService,
            IEmployeeRepository employeeRepository)
        {
            this.workflowTaskRepository = workflowTaskRepository;
            this.userResolverService = userResolverService;
            this.employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Method to complete a workflow Task
        /// </summary>
        /// <param name="workflowTaskIds">workflow Task identifiers</param>
        /// <returns>list of entity processing results</returns>
        public IList<EntityProcessingResult> CompleteWorkflowTasks(IEnumerable<long> workflowTaskIds)
        {
            Validator.ArgumentNotNull(workflowTaskIds, nameof(WorkflowTask));
            return this.workflowTaskRepository.CompleteWorkflowTasks(workflowTaskIds);
        }

        /// <summary>
        /// Method to create a workflow task
        /// </summary>
        /// <param name="taskItem">task item</param>
        /// <returns>a boolean value to indicate the success of task creation</returns>
        public bool CreateWorkflowTask(WorkflowTask taskItem)
        {
            bool isSuccess = false;

            if (taskItem.WorkflowTaskId != 0)
            {
                this.workflowTaskRepository.Update(taskItem);
            }
            else
            {
                this.workflowTaskRepository.Add(taskItem, true);
            }

            isSuccess = true;
            return isSuccess;
        }

        /// <summary>
        /// Gets all pending workflow tasks
        /// </summary>
        /// <returns>task reminder details</returns>
        public IQueryable<TaskReminderDetails> GetAllPendingWorkflowTasks()
        {
            return this.workflowTaskRepository.GetAllPendingWorkflowTasks();
        }

        /// <summary>
        /// Method to get pending or completed workflow tasks
        /// </summary>
        /// <param name="taskComplete">a boolean value which indicates whether pending or completed tasks have to be fetched</param>
        /// <returns>List of workflow task items</returns>
        public IQueryable<WorkflowTaskItem> GetWorkflowTasks(bool taskComplete)
        {
            string email = this.userResolverService.GetSimulatedUserEmail();

            var employeeId = this.employeeRepository.GetEmployeeIdByEmail(email);

            var tasks = this.workflowTaskRepository.GetWorkflowTasks(employeeId, taskComplete);
            return tasks;
        }
    }
}