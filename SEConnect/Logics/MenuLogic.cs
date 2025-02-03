//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="MenuLogic.cs" company="Microsoft">
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
    using System.Threading.Tasks;

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.DomainModels;
    using HP.ICE.Framework.Extensions;

    #endregion

    /// <summary>
    /// Class MenuLogic.
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.IMenuLogic" />
    public class MenuLogic : IMenuLogic
    {
        /// <summary>
        /// The application menu module repository
        /// </summary>
        private readonly IApplicationMenuModuleRepository applicationMenuModuleRepository;

        /// <summary>
        /// The application module repository
        /// </summary>
        private readonly IApplicationModuleRepository applicationModuleRepository;

        /// <summary>
        /// The menu repository
        /// </summary>
        private readonly IApplicationMenuRepository menuRepository;

        /// <summary>
        /// The application permission repository
        /// </summary>
        private readonly IApplicationPermissionRepository applicationPermissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuLogic" /> class.
        /// </summary>
        /// <param name="applicationPermissionRepository">The application permission repository.</param>
        /// <param name="menuRepository">The menu repository.</param>
        /// <param name="applicationMenuModuleRepository">The application menu module repository.</param>
        /// <param name="applicationModuleRepository">The application module repository.</param>
        public MenuLogic(
            IApplicationPermissionRepository applicationPermissionRepository,
            IApplicationMenuRepository menuRepository,
            IApplicationMenuModuleRepository applicationMenuModuleRepository,
            IApplicationModuleRepository applicationModuleRepository)
        {
            Validator.ArgumentNotNull(menuRepository, nameof(IApplicationMenuRepository));
            Validator.ArgumentNotNull(applicationPermissionRepository, nameof(IApplicationPermissionRepository));
            Validator.ArgumentNotNull(applicationMenuModuleRepository, nameof(IApplicationModuleRepository));
            this.menuRepository = menuRepository;
            this.applicationPermissionRepository = applicationPermissionRepository;
            this.applicationMenuModuleRepository = applicationMenuModuleRepository;
            this.applicationModuleRepository = applicationModuleRepository;
        }

        /// <summary>
        /// Gets all menus permitted to user.
        /// </summary>
        /// <returns>
        /// Task IEnumerable Menu
        /// </returns>
        public async Task<IEnumerable<ApplicationMenu>> GetAllMenusPermittedToUser()
        {
            var permittedMenus =
                await Task.FromResult(this.applicationPermissionRepository.GetLoggedInUserMenus().Result);

            return permittedMenus;
        }
    }
}