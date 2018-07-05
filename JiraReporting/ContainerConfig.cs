using System;
using Autofac;
using JiraReporting.Infrastructure.AutofacModules;

namespace JiraReporting
{
    /// <summary>
    /// Autofac container config
    /// </summary>
    public static class ContainerConfig
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <returns></returns>
        public static IContainer Configure(string nickname, string password, string jiraUri, DateTimeOffset lastReportDateTimeOffset)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new ServiceModule(nickname, password, jiraUri, lastReportDateTimeOffset));

            builder.RegisterModule(new AutomapperModule());

            builder.RegisterType<Application>().As<IApplication>();

            return builder.Build();
        }
    }
}