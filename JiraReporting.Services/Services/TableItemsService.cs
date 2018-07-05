using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;

namespace JiraReporting.Services.Services
{
    /// <summary>
    /// Table items service
    /// </summary>
    public class TableItemsService : ITableItemsService
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// The last report date time offset
        /// </summary>
        private readonly DateTimeOffset _lastReportDateTimeOffset;

        /// <summary>
        /// The check log service
        /// </summary>
        private readonly ICheckLogService _checkLogService;

        public TableItemsService(IMapper mapper, DateTimeOffset lastReportDateTimeOffset, ICheckLogService checkLogService)
        {
            _mapper = mapper;
            _lastReportDateTimeOffset = lastReportDateTimeOffset;
            _checkLogService = checkLogService;
        }

        /// <summary>
        /// Gets the impediments items list.
        /// </summary>
        /// <param name="impedimentIssues">The impediment issues.</param>
        /// <param name="teamName">Name of the team.</param>
        /// <returns></returns>
        public List<ImpedimentItemModel> GetImpedimentsList(List<AgileIssue> impedimentIssues, string teamName)
        {
            // Grouping issues by team members
            var goupedIssues = GroupIssuesByTeamMembers(impedimentIssues);

            var impedimentModelsList = _mapper.Map<Dictionary<string, List<AgileIssue>>, List<ImpedimentItemModel>>
                (goupedIssues, opt => opt.Items["teamName"] = teamName);

            return impedimentModelsList;
        }

        /// <summary>
        /// Gets the status items list.
        /// </summary>
        /// <param name="notImpedimentIssues">The not impediment issues.</param>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        public async Task<List<StatusItemModel>> GetStatusItemsList(List<AgileIssue> notImpedimentIssues, TeamModel team)
        {
            // Selecting issues which already have been done
            var workedIssues = notImpedimentIssues
                .Where(i => i.Fields.Progress.Progress != null && (i.Fields.Progress.Progress >= i.Fields.Progress.Total || i.Fields.Progress.Percent == 100))
                .ToList();

            //Selecting issues on which team members will work
            var willWorkIssues = notImpedimentIssues
                .Where(i => !workedIssues.Contains(i))
                .ToList();

            // Getting issues which was completed after last report creating
            workedIssues = workedIssues.Where(i => i.Fields.Updated != null && (i.Fields.Updated.Value - _lastReportDateTimeOffset).Minutes >= 0).ToList();

            // Grouping issues by teammembers
            var groupedWorkedIssues = GroupIssuesByTeamMembers(workedIssues);
            var groupedwillWorkIssues = GroupIssuesByTeamMembers(willWorkIssues);

            // Filtering the will work issues by estimate time (Getting issues on which developes is working in this moment)
            groupedwillWorkIssues = FilterWillWorkIssuesByEstimateTime(groupedwillWorkIssues);

            // Checking worked on issues that issues were completed after last report creating
            groupedWorkedIssues = await CheckingWorkedIssues(groupedWorkedIssues);

            // Creating the team member work issues models list
            var groupedWorkIssuesModel = CreateTeamMemberWorkIssuesModelList(team, groupedWorkedIssues, groupedwillWorkIssues);

            var statusItemModelsList = _mapper.Map<List<TeamMemberWorkIssuesModel>, List<StatusItemModel>>
                (groupedWorkIssuesModel, opt => opt.Items["teamName"] = team.TeamName);

            return statusItemModelsList;
        }

        /// <summary>
        /// Checkings the worked issues.
        /// </summary>
        /// <param name="groupedWorkedIssues">The grouped worked issues.</param>
        /// <returns></returns>
        private async Task<Dictionary<string, List<AgileIssue>>> CheckingWorkedIssues(Dictionary<string, List<AgileIssue>> groupedWorkedIssues)
        {
            var keys = groupedWorkedIssues.Keys.ToList();

            var tasks = keys.Select(async key =>
            {
                groupedWorkedIssues[key] = await _checkLogService.ChechWorkedOnIssues(groupedWorkedIssues[key], key);
            });

            await Task.WhenAll(tasks);

            return groupedWorkedIssues;
        }

        /// <summary>
        /// Filters the will work issues by estimate time.
        /// </summary>
        /// <param name="willWorkIssues">The will work issues.</param>
        /// <returns></returns>
        private Dictionary<string, List<AgileIssue>> FilterWillWorkIssuesByEstimateTime(Dictionary<string, List<AgileIssue>> willWorkIssues)
        {
            var keys = willWorkIssues.Keys.ToList();

            foreach (var key in keys)
            {
                if (willWorkIssues[key].Count > 5)
                {
                    willWorkIssues[key] = willWorkIssues[key]
                        .Where(iss => iss.Fields.TimeTracking.OriginalEstimateSeconds != null)
                        .ToList();
                }
            }

            return willWorkIssues;
        }

        /// <summary>
        /// Creates the team member work issues models list.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="groupedWorkedIssues">The grouped worked issues.</param>
        /// <param name="groupedwillWorkIssues">The groupedwill work issues.</param>
        /// <returns></returns>
        private List<TeamMemberWorkIssuesModel> CreateTeamMemberWorkIssuesModelList(TeamModel team, Dictionary<string, List<AgileIssue>> groupedWorkedIssues, Dictionary<string, List<AgileIssue>> groupedwillWorkIssues)
        {
            var groupedWorkIssuesModel = new List<TeamMemberWorkIssuesModel>();

            team.TeamMembersNames.AsParallel().ForAll(teamMember =>
            {
                var teamMemberWorkIssuesModel = new TeamMemberWorkIssuesModel
                {
                    Developer = teamMember.Replace(" (Contractor)", "").Replace(" (contractor)", ""),
                    GroupedWorkedIssues = groupedWorkedIssues.ContainsKey(teamMember)
                        ? groupedWorkedIssues[teamMember]
                        : new List<AgileIssue>(),
                    GroupedWillWorkIssues = groupedwillWorkIssues.ContainsKey(teamMember)
                        ? groupedwillWorkIssues[teamMember]
                        : new List<AgileIssue>()
                };

                groupedWorkIssuesModel.Add(teamMemberWorkIssuesModel);
            });

            return groupedWorkIssuesModel;
        }

        /// <summary>
        /// Groups the issues by team members.
        /// </summary>
        /// <param name="activeSprintIssues">The active sprint issues.</param>
        /// <returns></returns>
        private Dictionary<string, List<AgileIssue>> GroupIssuesByTeamMembers(List<AgileIssue> activeSprintIssues)
        {
            var groupedIssues = activeSprintIssues
                .GroupBy(i => i.Fields.Assignee.DisplayName)
                .ToDictionary(group => group.Key, group => group.ToList());

            return groupedIssues;
        }
    }
}