using System.Threading.Tasks;
using JiraReporting.Models;

namespace JiraReporting.Services.Interfaces
{
    /// <summary>
    /// Issues information service interface
    /// </summary>
    public interface IActiveSpintIssuesService
    {

        /// <summary>
        /// Gets the active spint issues model.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        Task<ActiveSpirntIssuesModel> GetActiveSpintIssuesModel(TeamModel team);
    }
}