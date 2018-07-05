using System.Collections.Generic;
using System.Threading.Tasks;
using JiraReporting.JiraClient.Entities;

namespace JiraReporting.Services.Interfaces
{
    /// <summary>
    /// Check log service interface
    /// </summary>
    public interface ICheckLogService
    {
        /// <summary>
        /// Checks the triage issues.
        /// </summary>
        /// <param name="issues">The issues.</param>
        /// <param name="teamMembers">The team members.</param>
        /// <returns></returns>
        Task<Dictionary<string, List<AgileIssue>>> CheckTriageIssues(List<AgileIssue> issues, List<string> teamMembers);

        /// <summary>
        /// Cheches the worked on issues.
        /// </summary>
        /// <param name="issues">The issues.</param>
        /// <param name="teamMemberName">Name of the team member.</param>
        /// <returns></returns>
        Task<List<AgileIssue>> ChechWorkedOnIssues(List<AgileIssue> issues, string teamMemberName);
    }
}
