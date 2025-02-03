// -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ManageRateScheduleLogic.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HP.ICE.BusinessLogic.Logics
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DomainModels;
    using DomainModels.ViewModel;
    using Framework.Data;
    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels.Custom;
    using HP.ICE.Framework.Extensions;

    #endregion

    /// <summary>
    /// The credit validation logic
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IManageRateScheduleLogic" />
    public class ManageRateScheduleLogic : IManageRateScheduleLogic
    {
        /// <summary>
        /// The credit validation repository
        /// </summary>
        private readonly IManageRateScheduleRepository manageRateScheduleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageRateScheduleLogic" /> class.
        /// </summary>
        /// <param name="mngRateScheduleRepository">The manage Rate Schedule Repository.</param>
        public ManageRateScheduleLogic(
            IManageRateScheduleRepository mngRateScheduleRepository)
        {
            Validator.ArgumentNotNull(mngRateScheduleRepository, nameof(mngRateScheduleRepository));

            this.manageRateScheduleRepository = mngRateScheduleRepository;
        }

        /// <summary>
        /// Get Searched Rate Schedule Details
        /// </summary>
        /// <param name="queryCriteria">the query  Criteria</param>
        /// <param name="filterRequest">the filter request</param>
        /// <returns>return Searched Rate Schedule Details</returns>
        public GridResult GetSearchedRateScheduleDetails(RateScheduleSearchCriteria queryCriteria, FilterRequest filterRequest)
        {
            return this.manageRateScheduleRepository.GetSearchedRateScheduleAdjustmentDetails(queryCriteria, filterRequest);
        }

        /// <summary>
        /// Gets  Adjust Rate Schedule Details for specific  rate.
        /// </summary>
        /// <param name="queryCriteria">The query criteria.</param>
        /// <param name="filterRequest">The filter request.</param>
        /// <returns>
        /// Adjust Rate Schedule Details
        /// </returns>
        public GridResult GetAdjustRateScheduleDetails(RateScheduleCriteria queryCriteria, FilterRequest filterRequest)
        {
            return this.manageRateScheduleRepository.GetAdjustRateScheduleDetails(queryCriteria, filterRequest);
        }

        /// <summary>
        /// Gets  Single Rate Schedule Details for specific  rate.
        /// </summary>
        /// <param name="sptpId">sales person target plan id.</param>
        /// <returns>
        /// return one Rate Schedule Details
        /// </returns>
        public RateScheduleAdjustmentDetails GetSingleRateScheduleDetails(long sptpId)
        {
            return this.manageRateScheduleRepository.GetSingleRateScheduleDetails(sptpId);
        }

        /// <summary>
        /// Get selected Rate Schedules.
        /// </summary>
        /// <param name="queryCriteria">The query criteria.</param>
        /// <param name="filter">The filter request.</param>
        /// <returns>
        /// Returns Rate Schedule Lists.
        /// </returns>
        public GridResult GetSelectRateSchedules(RateScheduleCriteria queryCriteria, FilterRequest filter)
        {
            Validator.ArgumentNotNull(queryCriteria, nameof(queryCriteria));
            Validator.ArgumentNotNull(filter, nameof(filter));

            return this.manageRateScheduleRepository.GetRateSchedules(queryCriteria, filter);
        }

        /// <summary>
        /// Create Rate Schedule Adjustment Rate Schedule.
        /// </summary>
        ///  <param name="createCriteria">The sear query Criteria.</param>
        /// <returns>
        /// Returns  Rate Schedules Adjustment
        /// </returns>
        public CreateRateScheduleAdjustment CreateRateScheduleDetails(CreateRateScheduleCriteria createCriteria)
        {
            Validator.ArgumentNotNull(createCriteria, nameof(createCriteria));
            return this.manageRateScheduleRepository.CreateRateScheduleDetails(createCriteria);
        }

        /// <summary>
        /// get rate schedule and adjusted rate schedule
        /// </summary>
        /// <param name="sptpid">sales person target plan id</param>
        /// <returns>list of rate schedule and adjusted rate schedule</returns>
        public IEnumerable<RateScheduleAdjustmentViewModel> GetRateScheduleAndAdjustedRateSchedule(long sptpid)
        {
            return this.manageRateScheduleRepository.GetRateScheduleAndAdjustedRateSchedule(sptpid.ToString());
        }

        /// <summary>
        /// updates rate schedule adjustment. enter new entry if not exists, update / delete
        /// </summary>
        /// <param name="updateRateSchedule">rate schedule adjustment detail</param>
        /// <returns>list of entity processing result</returns>
        public List<EntityProcessingResult> AdjustRateSchedule(List<RateScheduleAdjustmentViewModel> updateRateSchedule)
        {
            var entityProcessingResults = new List<EntityProcessingResult>();
            Validator.ArgumentNotNull(updateRateSchedule, nameof(updateRateSchedule));
            if (updateRateSchedule.Count >= 0)
            {
                var sptpId = updateRateSchedule[0].SalesPersonTargetPlanId;
                var regionalIncentivePlanMetricId = updateRateSchedule[0].RegionalIncentivePlanMetricId;
                var entities = this.manageRateScheduleRepository.Get(x => x.SalesPersonTargetPlanId == sptpId && x.IsDeleted == false).ToList();
                if (regionalIncentivePlanMetricId == null && entities.Count == 0)
                {
                    var entity = new RateScheduleAdjustment
                    {
                        SalesPersonTargetPlanId = updateRateSchedule[0].SalesPersonTargetPlanId,
                        RateScheduleId = updateRateSchedule[0].AdjustedRateScheduleId.Value,
                        RegionalIncentivePlanId = updateRateSchedule[0].RegionalIncentivePlanId,
                        RegionalIncentivePlanMetricId = null,
                        SalesLetterId = updateRateSchedule[0].SalesLetterId.Value
                    };
                    this.manageRateScheduleRepository.Add(entity);
                    entityProcessingResults.Add(new EntityProcessingResult { Id = entity.RateScheduleAdjustmentId });
                }
                else
                {
                    foreach (var rateSchedueleAdjustment in updateRateSchedule)
                    {
                        var entity = entities.FirstOrDefault(x => x.RegionalIncentivePlanId == rateSchedueleAdjustment.RegionalIncentivePlanId && x.RegionalIncentivePlanMetricId == rateSchedueleAdjustment.RegionalIncentivePlanMetricId);
                        if (rateSchedueleAdjustment.AdjustedRateScheduleId == null && entity != null)
                        {
                            entity.IsDeleted = true;
                            this.manageRateScheduleRepository.Update(entity);
                            entityProcessingResults.Add(new EntityProcessingResult { IsDeleted = true, Id = entity.RateScheduleAdjustmentId });
                        }
                        else if (rateSchedueleAdjustment.AdjustedRateScheduleId != null && entity == null)
                        {
                            entity = new RateScheduleAdjustment
                            {
                                SalesPersonTargetPlanId = rateSchedueleAdjustment.SalesPersonTargetPlanId,
                                RateScheduleId = rateSchedueleAdjustment.AdjustedRateScheduleId.Value,
                                RegionalIncentivePlanId = rateSchedueleAdjustment.RegionalIncentivePlanId,
                                RegionalIncentivePlanMetricId = rateSchedueleAdjustment.RegionalIncentivePlanMetricId,
                                SalesLetterId = rateSchedueleAdjustment.SalesLetterId.Value
                            };
                            this.manageRateScheduleRepository.Add(entity);
                            entityProcessingResults.Add(new EntityProcessingResult { Id = entity.RateScheduleAdjustmentId });
                        }
                        else if (rateSchedueleAdjustment.AdjustedRateScheduleId != null && rateSchedueleAdjustment.AdjustedRateScheduleId != entity.RateScheduleAdjustmentId)
                        {
                            entity.RateScheduleId = rateSchedueleAdjustment.AdjustedRateScheduleId.Value;
                            this.manageRateScheduleRepository.Update(entity);
                            entityProcessingResults.Add(new EntityProcessingResult { Id = entity.RateScheduleAdjustmentId });
                        }
                    }
                }
            }

            return entityProcessingResults;
        }
    }
}