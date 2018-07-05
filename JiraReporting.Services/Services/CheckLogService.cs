using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraReporting.JiraClient;
using JiraReporting.JiraClient.Entities;
using JiraReporting.Services.Interfaces;

namespace JiraReporting.Services.Services
{
    /// <summary>
    /// CheckLog service
    /// </summary>
    /// <seealso cref="ICheckLogService" />
    public class CheckLogService : ICheckLogService
    {
        /// <summary>
        /// The last report date time offset
        /// </summary>
        private readonly DateTimeOffset _lastReportDateTimeOffset;

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
        /// Initializes a new instance of the <see cref="CheckLogService"/> class.
        /// </summary>
        /// <param name="nickname">The nickname.</param>
        /// <param name="password">The password.</param>
        /// <param name="jiraUri">The jira URI.</param>
        /// <param name="lastReportDateTimeOffset">The last report date time offset.</param>
        public CheckLogService(string nickname, string password, string jiraUri, DateTimeOffset lastReportDateTimeOffset)
        {
            _lastReportDateTimeOffset = lastReportDateTimeOffset;
            _nickName = nickname;
            _password = password;
            _jiraUri = jiraUri;
        }

        /// <summary>
        /// Checks the triage issues.
        /// </summary>
        /// <param name="issues">The issues.</param>
        /// <param name="teamMembers">The team members.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Dictionary<string, List<AgileIssue>>> CheckTriageIssues(List<AgileIssue> issues, List<string> teamMembers)
        {
            // Initialize dictionary
            var triageIssues = new Dictionary<string, List<AgileIssue>>();

            foreach (var teamMember in teamMembers)
            {
                triageIssues.Add(teamMember, new List<AgileIssue>());
            }

            // Checking is teammebers logged time to issue
            var tasks = issues.Select(async issue => new Tuple<AgileIssue, Changelog>(issue, await GetCheckLog(Convert.ToInt32(issue.Id))));
            var checkLogs = await Task.WhenAll(tasks);

            teamMembers.AsParallel().ForAll(teamMember =>
            {
                var teamMemberLoggedTimeLogs = checkLogs.
                    Where(checkLog => checkLog.Item2.Historiess.
                        Any(h => h.Author.DisplayName.Contains(teamMember) && h.Items.Any(hi => hi.Field == "timespent"))).
                    Select(checkLog => checkLog.Item1);

                triageIssues[teamMember].AddRange(teamMemberLoggedTimeLogs);
            });

            return triageIssues;
        }

        /// <summary>
        /// Cheches the worked on issues.
        /// </summary>
        /// <param name="issues">The issues.</param>
        /// <param name="teamMemberName">Name of the team member.</param>
        /// <returns></returns>
        public async Task<List<AgileIssue>> ChechWorkedOnIssues(List<AgileIssue> issues, string teamMemberName)
        {
            var workedIssues = new List<AgileIssue>();

            var tasks = issues.Select(async issue =>
            {
                var checkLog = await GetCheckLog(Convert.ToInt32(issue.Id));
                var isLoggedLessThatOneDayAgo = checkLog.Historiess.Any(h =>
                    (h.Created - _lastReportDateTimeOffset).Minutes >= 0 &&
                    h.Author.DisplayName.Contains(teamMemberName));

                if (isLoggedLessThatOneDayAgo)
                {
                    lock (workedIssues)
                    {
                        workedIssues.Add(issue);
                    }
                }
            });

            await Task.WhenAll(tasks);

            return workedIssues;
        }

        /// <summary>
        /// Gets the check log.
        /// </summary>
        /// <param name="issuesId">The issues identifier.</param>
        /// <returns></returns>
        private async Task<Changelog> GetCheckLog(int issuesId)
        {
            using (var client = new JiraClient.JiraClient(new Uri(_jiraUri)))
            {
                client.SetBasicAuthentication(_nickName, _password);

                var checkLogSearchResult = await client.Agile.GetChangelogAsync(issuesId);

                return checkLogSearchResult.Changelog;
            }
        }
    }
}
