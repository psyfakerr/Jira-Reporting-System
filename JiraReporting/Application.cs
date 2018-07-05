using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JiraReporting.Models;
using JiraReporting.Services.Interfaces;
using Newtonsoft.Json;

namespace JiraReporting
{
    /// <summary>
    ///
    /// </summary>
    public class Application : IApplication
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly IReportsService _service;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public Application(IReportsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Runs programm asynchronously.
        /// </summary>
        /// <returns></returns>k
        public async Task RunAsync()
        {
            var teams = JsonConvert.DeserializeObject<List<TeamModel>>(
                File.ReadAllText(@"..\..\teams.json"));
            var triageTeam = JsonConvert.DeserializeObject<List<TeamModel>>(
                File.ReadAllText(@"..\..\triage.json"));


            var recipients = JsonConvert.DeserializeObject<List<RecipientModel>>(
                File.ReadAllText(@"..\..\recipients.json"));
           
            await _service.CreateReportForAllTeams(teams, triageTeam, recipients);
        }
    }
}