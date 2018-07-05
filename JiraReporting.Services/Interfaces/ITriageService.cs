using System.Collections.Generic;
using System.Threading.Tasks;
using JiraReporting.Models;

namespace JiraReporting.Services.Interfaces
{
    /// <summary>
    /// Triage service interface
    /// </summary>
    public interface ITriageService
    {
        /// <summary>
        /// Gets the triage items list.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        Task<List<TriageItemModel>> GetTriageItemsList(TeamModel team);
    }
}
