using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraReporting.JiraClient;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;

namespace JiraReporting.Services.Services
{
    /// <summary>
    /// Service for getting information about issues
    /// </summary>
    public class ActiveSpintIssuesService : IActiveSpintIssuesService
    {
        /// <summary>
        /// The nick name
        /// </summary>
        private readonly string _nickName;

        /// <summary>
        /// The password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The jira URI
        /// </summary>
        private readonly string _jiraUri;

        /// <summary>
        /// The impeded issue types
        /// </summary>
        private readonly List<string> _impededIssueTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveSpintIssuesService"/> class.
        /// </summary>
        public ActiveSpintIssuesService(string nickname, string password, string jiraUri)
        {
            _nickName = nickname;
            _password = password;
            _jiraUri = jiraUri;

            _impededIssueTypes = new List<string>()
            {
                "ON HOLD",
                "DEVELOPMENT IMPEDED",
                "TESTING IMPEDED"
            };
        }

        /// <summary>
        /// Gets the impediments list.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        public async Task<ActiveSpirntIssuesModel> GetActiveSpintIssuesModel(TeamModel team)
        {
            var activeSprintIssues = await GetAllTeamIssuesInCurrentSprint(team.TeamMembersNames, team.ProjectName, team.ProjectAgileBoardName);

            // Selecting only impeded issues
            var impedimentIssues = activeSprintIssues
                .Where(i => _impededIssueTypes.Contains(i.Fields.Status.Name))
                .ToList();

            // Selecting not impeded issues
            var notImpedimentIssues = activeSprintIssues
                .Where(i => !_impededIssueTypes.Contains(i.Fields.Status.Name))
                .ToList();

            var activeSpirntIssuesModel = new ActiveSpirntIssuesModel()
            {
                ImpedimentIssues = impedimentIssues,
                NotImpedimentIssues = notImpedimentIssues
            };

            return activeSpirntIssuesModel;
        }

        /// <summary>
        /// Gets all team issues in current sprint.
        /// </summary>
        /// <param name="teamMembersNames">The team members names.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="projectAgileBoardName">Name of the project agile board.</param>
        /// <returns></returns>
        private async Task<List<AgileIssue>> GetAllTeamIssuesInCurrentSprint(IEnumerable<string> teamMembersNames, string projectName, string projectAgileBoardName)
        {
            using (var client = new JiraClient.JiraClient(new Uri(_jiraUri)))
            {

                client.SetBasicAuthentication(_nickName, _password);

                // Getting projectId by project name
                var projects = await client.Project.GetAllAsync();
                var projectId = projects.First(p => p.Name == projectName).Key;

                // Getting project boardId by projectid
                var searchResult = await client.Agile.GetBoardsAsync(null, null, projectId);
                var boardId = searchResult.First(b => b.Name == projectAgileBoardName).Id;

                // Getting active sprint in current board
                var sprintId = client.Agile.GetSprintsAsync(boardId, "active").Result.First().Id;

                var msxResultsCount = new Page() { StartAt = 1, MaxResults = 1000 };
                var activeSprintIssues = new List<AgileIssue>();

                while (activeSprintIssues.Count == 0 || activeSprintIssues.Count % 1000 == 0)
                {
                    var issues = await client.Agile.GetIssuesInSprintAsync(boardId, sprintId, msxResultsCount);

                    activeSprintIssues.AddRange(issues);
                    msxResultsCount.StartAt += 1000;
                }

                // Selecting issues which was assigned on team members
                var teamActiveSprintIssues = activeSprintIssues
                    .Where(i => i.Fields.Assignee != null && teamMembersNames.Contains(i.Fields.Assignee.DisplayName))
                    .ToList();

                return teamActiveSprintIssues;
            }
        }
    }
}