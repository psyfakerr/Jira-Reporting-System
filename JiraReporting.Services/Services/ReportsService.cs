using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;

namespace JiraReporting.Services.Services
{
    /// <summary>
    /// Service for making reports
    /// </summary>
    public class ReportsService : IReportsService
    {
        /// <summary>
        /// The issues information service
        /// </summary>
        private readonly IActiveSpintIssuesService _activeSpintIssuesService;

        /// <summary>
        /// The table items service
        /// </summary>
        private readonly ITableItemsService _tableItemsService;

        /// <summary>
        /// The table items service
        /// </summary>
        private readonly IEmailSenderService _emailSenderService;

        /// <summary>
        /// The triage service
        /// </summary>
        private readonly ITriageService _triageService;

        /// <summary>
        /// The triage service
        /// </summary>
        /// <param name="activeSpintIssuesService">The issues information service.</param>
        /// <param name="tableItemsService">The table items service.</param>
        /// <param name="emailSenderService">The outlook email service.</param>
        /// <param name="triageService">The triage service.</param>
        public ReportsService(IActiveSpintIssuesService activeSpintIssuesService, ITableItemsService tableItemsService, IEmailSenderService emailSenderService, ITriageService triageService)
        {
            _activeSpintIssuesService = activeSpintIssuesService;
            _tableItemsService = tableItemsService;
            _emailSenderService = emailSenderService;
            _triageService = triageService;
        }

        /// <summary>
        /// Creates the report for all teams.
        /// </summary>
        /// <param name="teams">The teams.</param>
        /// <param name="triageTeams">The triage teams.</param>
        /// <param name="recipients">The recipients.</param>
        /// <returns></returns>
        public async Task CreateReportForAllTeams(List<TeamModel> teams, List<TeamModel> triageTeams, List<RecipientModel> recipients)
        {
            var impededItems = new List<ImpedimentItemModel>();
            var statusItems = new List<StatusItemModel>();
            var triageItems = new List<TriageItemModel>();

            var tasks = teams.Select(async team =>
            {
                var activeSprintIssuesModel = await _activeSpintIssuesService.GetActiveSpintIssuesModel(team);

                var teamImpededItems =
                    _tableItemsService.GetImpedimentsList(activeSprintIssuesModel.ImpedimentIssues, team.TeamName);
                lock (impededItems)
                {
                    impededItems.AddRange(teamImpededItems);
                }

                var teamStatusItems = await _tableItemsService.GetStatusItemsList(activeSprintIssuesModel.NotImpedimentIssues, team);

                lock (statusItems)
                {
                    statusItems.AddRange(teamStatusItems);
                }
            });

            await Task.WhenAll(tasks);

            var triageTasks = triageTeams.Select(async team =>
            {
                var teamTriageItems = await _triageService.GetTriageItemsList(team);
                lock (triageItems)
                {
                    triageItems.AddRange(teamTriageItems);
                }
            });

            await Task.WhenAll(triageTasks);
            
            _emailSenderService.FormationTablesForLetter(impededItems, statusItems, recipients, triageItems);
        }
    }
}