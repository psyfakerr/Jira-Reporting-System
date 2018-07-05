using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using JiraReporting.Services.Interfaces;
using JiraReporting.Services.OutLookServices;
using JiraReporting.Services.Services;

namespace JiraReporting.Infrastructure.AutofacModules
{
    /// <summary>
    /// Autofac module for registring services level dependencies
    /// </summary>
    /// <seealso cref="System.Reflection.Module" />
    public class ServiceModule : Module
    {
        /// <summary>
        /// The nickname
        /// </summary>
        private readonly string _nickname;

        /// <summary>
        /// The password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The jira URI
        /// </summary>
        private readonly string _jiraUri;

        /// <summary>
        /// The last report date time offset
        /// </summary>
        private readonly DateTimeOffset _lastReportDateTimeOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceModule" /> class.
        /// </summary>
        /// <param name="nickname">The nickname.</param>
        /// <param name="password">The password.</param>
        /// <param name="jiraUri">The jira URI.</param>
        /// <param name="lastReportDateTimeOffset">The last report date time offset.</param>
        public ServiceModule(string nickname, string password, string jiraUri, DateTimeOffset lastReportDateTimeOffset)
        {
            _nickname = nickname;
            _password = password;
            _jiraUri = jiraUri;
            _lastReportDateTimeOffset = lastReportDateTimeOffset;
        }
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReportsService>().As<IReportsService>();
            builder.RegisterType<EmailSenderService>().As<IEmailSenderService>();

            builder.RegisterType<ActiveSpintIssuesService>().As<IActiveSpintIssuesService>().WithParameters(new List<Parameter>()
            {
                new NamedParameter("nickname", _nickname),
                new NamedParameter("password", _password),
                new NamedParameter("jiraUri", _jiraUri)
            });

            builder.RegisterType<TableItemsService>().As<ITableItemsService>().WithParameters(new List<Parameter>()
            {
                new NamedParameter("lastReportDateTimeOffset", _lastReportDateTimeOffset)
            });

            builder.RegisterType<TriageService>().As<ITriageService>().WithParameters(new List<Parameter>()
            {
                new NamedParameter("nickname", _nickname),
                new NamedParameter("password", _password),
                new NamedParameter("jiraUri", _jiraUri)
            });

            builder.RegisterType<CheckLogService>().As<ICheckLogService>().WithParameters(new List<Parameter>()
            {
                new NamedParameter("nickname", _nickname),
                new NamedParameter("password", _password),
                new NamedParameter("jiraUri", _jiraUri),
                new NamedParameter("lastReportDateTimeOffset", _lastReportDateTimeOffset)
            });
        }
    }
}