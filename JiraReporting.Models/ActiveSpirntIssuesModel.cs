using System.Collections.Generic;
using JiraReporting.JiraClient.Entities;

namespace JiraReporting.Models
{
    /// <summary>
    /// Model which contains all active sprint issues
    /// </summary>
    public class ActiveSpirntIssuesModel
    {
        /// <summary>
        /// Gets or sets the impediment issues.
        /// </summary>
        /// <value>
        /// The impediment issues.
        /// </value>
        public List<AgileIssue> ImpedimentIssues { get; set; }

        /// <summary>
        /// Gets or sets the not impediment issues.
        /// </summary>
        /// <value>
        /// The not impediment issues.
        /// </value>
        public List<AgileIssue> NotImpedimentIssues { get; set; }
    }
}