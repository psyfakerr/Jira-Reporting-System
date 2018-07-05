using System.Collections.Generic;
using JiraReporting.JiraClient.Entities;

namespace JiraReporting.Models
{
    /// <summary>
    /// Model for saving gouped work issues
    /// </summary>
    public class TeamMemberWorkIssuesModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Developer { get; set; }

        /// <summary>
        /// Gets or sets the goupedwill work issues.
        /// </summary>
        /// <value>
        /// The goupedwill work issues.
        /// </value>
        public List<AgileIssue> GroupedWillWorkIssues { get; set; }
        
        /// <summary>
        /// Gets or sets the gouped worked issues.
        /// </summary>
        /// <value>
        /// The gouped worked issues.
        /// </value>
        public List<AgileIssue> GroupedWorkedIssues { get; set; }
    }
}