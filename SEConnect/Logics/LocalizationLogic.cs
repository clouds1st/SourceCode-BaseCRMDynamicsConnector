//  -------------------------------------------------------------------------------------------------------------------- 
// <copyright file="LocalizationLogic.cs" company="Microsoft">
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

    using HP.ICE.BusinessLogic.Interfaces;
    using HP.ICE.DataAccess.Interfaces;
    using HP.ICE.Framework;

    #endregion

    /// <summary>
    /// The application permission logic
    /// </summary>
    /// <seealso cref="HP.ICE.BusinessLogic.Interfaces.ILocalizationLogic" />
    public class LocalizationLogic : ILocalizationLogic
    {
        /// <summary>
        /// The localization repository
        /// </summary>
        private readonly ILocalizationRepository localizationRepository;

        /// <summary>
        /// The user resolver service
        /// </summary>
        private readonly IUserResolverService userResolverService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationLogic"/> class.
        /// </summary>
        /// <param name="localizationRepository">The localization repository.</param>
        /// <param name="userResolverService">The user resolver service.</param>
        public LocalizationLogic(
            ILocalizationRepository localizationRepository,
            IUserResolverService userResolverService)
        {
            this.userResolverService = userResolverService;
            this.localizationRepository = localizationRepository;
        }

        /// <summary>
        /// Gets the localization values based on user locale.
        /// </summary>
        /// <param name="module">module name</param>
        /// <param name="locale">local name</param>
        /// <returns>Returns localization values</returns>
        public Dictionary<string, string> GetLocalizationValues(string module, int locale)
        {
            Dictionary<string, string> dictLocale = new Dictionary<string, string>();
            var localeValues = this.localizationRepository.GetLocalizationValues(module, locale);

            if (localeValues.Result != null)
            {
                foreach (var resource in localeValues.Result)
                {
                    dictLocale.Add(resource.Key, resource.Value);
                }
            }

            return dictLocale;
        }
    }
}