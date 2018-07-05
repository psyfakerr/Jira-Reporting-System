using System.Collections.Generic;
using System.Threading.Tasks;
using JiraReporting.Models;

namespace JiraReporting.Services.Interfaces
{
    /// <summary>
    /// Reposrt service interface
    /// </summary>
    public interface IReportsService
    {
        /// <summary>
        /// Creates the report for all teams.
        /// </summary>
        /// <param name="teams">The teams.</param>
        /// <param name="triageTeams">The triage teams.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns></returns>
        Task CreateReportForAllTeams(List<TeamModel> teams, List<TeamModel> triageTeams, List<RecipientModel> recipients);
    }
}
