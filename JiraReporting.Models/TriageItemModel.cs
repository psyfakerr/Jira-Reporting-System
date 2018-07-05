using System.Collections.Generic;

namespace JiraReporting.Models
{
    public class TriageItemModel
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
        /// Gets or sets the impediment issue description models.
        /// </summary>
        /// <value>
        /// The impediment issue description models.
        /// </value>
        public List<IssueDescriptionModel> ImpedimentIssueDescriptionModels { get; set; }

        /// <summary>
        /// Gets or sets the worked issue description models.
        /// </summary>
        /// <value>
        /// The worked issue description models.
        /// </value>
        public List<IssueDescriptionModel> WorkedIssueDescriptionModels { get; set; }

        /// <summary>
        /// Gets or sets the will work on issue description models.
        /// </summary>
        /// <value>
        /// The will work on issue description models.
        /// </value>
        public List<IssueDescriptionModel> WillWorkOnIssueDescriptionModels { get; set; }
    }
}
