//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="IncentivePlanMetricEntityProcessor.cs" company="Microsoft">
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//  OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// -------------------------------------------------------------------------------------------------------------------- 

namespace HP.ICE.BusinessLogic.EntityProcessors
{
    #region Namespaces

    using DocumentFormat.OpenXml.Spreadsheet;

    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework;
    using HP.ICE.Framework.Extensions;

    #endregion

    /// <summary>
    /// The incentive plan entity processor
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.EntityProcessors.EntityProcessorBase" />
    /// <seealso cref="HP.ICE.BusinessLogic.IEntityProcessor" />
    public class IncentivePlanMetricEntityProcessor : EntityProcessorBase, IEntityProcessor
    {
        /// <summary>
        /// The incentive plan upload processing repository
        /// </summary>
        private readonly IIncentivePlanUploadProcessingRepository incentivePlanUploadProcessingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncentivePlanMetricEntityProcessor" /> class.
        /// </summary>
        /// <param name="userResolverService">The user resolver service.</param>
        /// <param name="incentivePlanUploadProcessingRepository">The incentive plan upload processing repository.</param>
        public IncentivePlanMetricEntityProcessor(
            IUserResolverService userResolverService,
            IIncentivePlanUploadProcessingRepository incentivePlanUploadProcessingRepository)
            : base(userResolverService)
        {
            Validator.ArgumentNotNull(
                incentivePlanUploadProcessingRepository,
                nameof(incentivePlanUploadProcessingRepository));

            this.incentivePlanUploadProcessingRepository = incentivePlanUploadProcessingRepository;
        }

        /// <summary>
        /// Processes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="rowActionInd">The row action ind.</param>
        /// <param name="identityColumnName">Name of the identity column.</param>
        /// <param name="identityCell">The identity cell.</param>
        /// <returns>
        /// String Value.
        /// </returns>
        public string ProcessEntity(
            object entity,
            string rowActionInd,
            string identityColumnName = null,
            Cell identityCell = null)
        {
            IncentivePlanMetric incentivePlanMetric = entity as IncentivePlanMetric;

            Validator.ArgumentNotNull(incentivePlanMetric, nameof(incentivePlanMetric));
            Validator.ArgumentNotNull(rowActionInd, nameof(rowActionInd));

            this.AssignUserNameBasedAuditFields(incentivePlanMetric);

            var uploadResult = this.incentivePlanUploadProcessingRepository.UploadIncentivePlanMetric(
                rowActionInd,
                incentivePlanMetric);
            if (uploadResult != null)
            {
                incentivePlanMetric.IncentivePlanMetricId = uploadResult.IncentivePlanMetricId;
            }

            return this.ProcessUploadResult(uploadResult);
        }
    }
}