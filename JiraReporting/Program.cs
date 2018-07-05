using System;
using System.Configuration;
using System.IO;
using Autofac;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace JiraReporting
{
    /// <summary>
    /// Main program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            
            var nickname = ConfigurationManager.AppSettings.Get("NickName");
            var password = ConfigurationManager.AppSettings.Get("Password");
            var jiraUri = ConfigurationManager.AppSettings.Get("JiraUri");
            var lastReportDateTimeOffset = JsonConvert.DeserializeObject<DateTimeOffset>(
                File.ReadAllText(@"..\..\lastReportDateTime.json"));

            app.OnExecute(() =>
            {
                var container = ContainerConfig.Configure(nickname, password, jiraUri, lastReportDateTimeOffset);
                using (var scope = container.BeginLifetimeScope())
                {
                    var application = scope.Resolve<IApplication>();
                    application.RunAsync().GetAwaiter().GetResult();
                }

                var json = JsonConvert.SerializeObject(DateTimeOffset.UtcNow - TimeSpan.FromHours(10));

                //write lastReportDateTimeOffset to file
                File.WriteAllText(@"..\..\lastReportDateTime.json", json);
            });

            return app.Execute(args);
        }
    }
}