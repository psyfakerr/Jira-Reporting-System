using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JiraReporting.JiraClient;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;

namespace JiraReporting.Services.Services
{
    public class TriageService : ITriageService
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// The client
        /// </summary>
        private readonly IJiraClient _client;

        /// <summary>
        /// The check log service
        /// </summary>
        private readonly ICheckLogService _checkLogService;

        /// <summary>
        /// The impeded issue types
        /// </summary>
        private readonly List<string> _impededIssueTypes;

        public TriageService(string nickname, string password, string jiraUri, ICheckLogService checkLogService, IMapper mapper)
        {
            _checkLogService = checkLogService;
            _mapper = mapper;
            _client = new JiraClient.JiraClient(new Uri(jiraUri));
            _client.SetBasicAuthentication(nickname, password);

            _impededIssueTypes = new List<string>()
            {
                "ON HOLD",
                "DEVELOPMENT IMPEDED",
                "TESTING IMPEDED"
            };
        }

        /// <summary>
        /// Gets the status items list.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        public async Task<List<TriageItemModel>> GetTriageItemsList(TeamModel team)
        {
            var triageIssues = await GetAllTriageIssues(team);

            var groupedTriageIssues = await _checkLogService.CheckTriageIssues(triageIssues, team.TeamMembersNames.ToList());

            var groupedTriageIssuesModel = await CreateTriageItemModelsList(team, groupedTriageIssues);

            var statusItemModelsList = _mapper.Map<List<TeamMemberTriageIssuesModel>, List<TriageItemModel>>
                (groupedTriageIssuesModel, opt => opt.Items["teamName"] = team.TeamName);

            return statusItemModelsList;
        }

        /// <summary>
        /// Creates the triage item models list.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="groupedTriageIssues">The grouped triage issues.</param>
        /// <returns></returns>
        private async Task<List<TeamMemberTriageIssuesModel>> CreateTriageItemModelsList(TeamModel team, Dictionary<string, List<AgileIssue>> groupedTriageIssues)
        {
            var groupedTriageIssuesModel = new List<TeamMemberTriageIssuesModel>();

            var tasks = team.TeamMembersNames.AsParallel().Select(async teamMember =>
            {
                var issue = groupedTriageIssues.ContainsKey(teamMember)
                    ? groupedTriageIssues[teamMember] : new List<AgileIssue>();

                var teamMemberTriageIssuesModel = new TeamMemberTriageIssuesModel
                {
                    Developer = teamMember.Replace(" (Contractor)", "").Replace(" (contractor)", ""),
                };

                teamMemberTriageIssuesModel = await FilterIssuesByType(teamMemberTriageIssuesModel, issue);

                groupedTriageIssuesModel.Add(teamMemberTriageIssuesModel);
            });

            await Task.WhenAll(tasks);

            return groupedTriageIssuesModel;
        }

        /// <summary>
        /// Filters the type of the issues by.
        /// </summary>
        /// <param name="triageIssues">The team member triage issues model.</param>
        /// <param name="issues">The issues.</param>
        /// <returns></returns>
        private async Task<TeamMemberTriageIssuesModel> FilterIssuesByType(TeamMemberTriageIssuesModel triageIssues, List<AgileIssue> issues)
        {
            triageIssues.GropedImpedimentIssues = issues
            .Where(i => _impededIssueTypes.Contains(i.Fields.Status.Name))
                .ToList();

            triageIssues.GroupedWorkedIssues = issues
                .Where(i => i.Fields.Progress.Progress != null && (i.Fields.Progress.Progress >= i.Fields.Progress.Total || i.Fields.Progress.Percent == 100))
                .ToList();

            triageIssues.GroupedWillWorkIssues = issues
                .Where(i => !triageIssues.GroupedWorkedIssues.Contains(i) && !triageIssues.GropedImpedimentIssues.Contains(i))
                .ToList();
            // Checking will work issues on estimate present
            triageIssues.GroupedWillWorkIssues = FilterWillWorkIssuesByEstimateTime(triageIssues.GroupedWillWorkIssues);

            // Checking worked on issues that issues were completed after last report creating
            triageIssues.GroupedWorkedIssues = await
                _checkLogService.ChechWorkedOnIssues(triageIssues.GroupedWorkedIssues,
                    triageIssues.Developer);

            return triageIssues;
        }

        //    return groupedWorkedIssues;
        /// <summary>
        /// Gets all triage issues.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>

        private async Task<List<AgileIssue>> GetAllTriageIssues(TeamModel team)
        {
            // Getting projectId by project name
            var projects = await _client.Project.GetAllAsync();
            var projectId = projects.First(p => p.Name == team.ProjectName).Key;

            // Getting project boardId by projectid
            var searchResult = await _client.Agile.GetBoardsAsync(null, null, projectId);
            var boardId = searchResult.First(b => b.Name == team.ProjectAgileBoardName).Id;

            var issues = await _client.Agile.GetIssuesOnBoardAsync(boardId, new Page()
            {
                StartAt = 0,
                MaxResults = 1000
            });

            return issues.ToList();
        }
        /// <summary>
        /// Filters the will work issues by estimate time.
        /// </summary>
        /// <param name="willWorkIssues">The will work issues.</param>
        /// <returns></returns>
        private List<AgileIssue> FilterWillWorkIssuesByEstimateTime(List<AgileIssue> willWorkIssues)
        {
            if (willWorkIssues.Count > 5)
            {
                willWorkIssues = willWorkIssues
                    .Where(iss => iss.Fields.TimeTracking.TimeSpentSeconds
                    != null)
                    .ToList();
            }

            return willWorkIssues;
        }
    }
}
