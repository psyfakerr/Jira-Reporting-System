using System.Collections.Generic;

namespace JiraReporting.Models
{
    public class StatusItemModel
    {   
        /// <summary>
        /// Gets or sets the team.
        /// </summary>
        /// <value>
        /// The team.
        /// </value>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the developer.
        /// </summary>
        /// <value>
        /// The developer.
        /// </value>
        public string Developer { get; set; }

        /// <summary>
        /// Gets or sets the completed issue description models.
        /// </summary>
        /// <value>
        /// The completed issue description models.
        /// </value>
        public List<IssueDescriptionModel> CompletedIssueDescriptionModels { get; set; }

        /// <summary>
        /// Gets or sets the future issue description models.
        /// </summary>
        /// <value>
        /// The future issue description models.
        /// </value>
        public List<IssueDescriptionModel> FutureIssueDescriptionModels { get; set; }
    }
}