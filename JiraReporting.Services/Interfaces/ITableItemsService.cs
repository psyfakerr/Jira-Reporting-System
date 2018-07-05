using System.Collections.Generic;
using System.Threading.Tasks;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Models;

namespace JiraReporting.Services.Interfaces
{
    /// <summary>
    /// Table items service interface
    /// </summary>
    public interface ITableItemsService
    {
        /// <summary>
        /// Gets the impediments list.
        /// </summary>
        /// <param name="impedimentIssues">The impediment issues.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <returns></returns>
        List<ImpedimentItemModel> GetImpedimentsList(List<AgileIssue> impedimentIssues, string teamName);

        /// <summary>
        /// Gets the status items list.
        /// </summary>
        /// <param name="notImpedimentIssues">The not impediment issues.</param>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        Task<List<StatusItemModel>> GetStatusItemsList(List<AgileIssue> notImpedimentIssues, TeamModel team);
    }
}