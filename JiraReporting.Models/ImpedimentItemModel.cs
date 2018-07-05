using System.Collections.Generic;

namespace JiraReporting.Models
{
    /// <summary>
    /// Impediment item model
    /// </summary>
    public class ImpedimentItemModel
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
        /// Gets or sets the impedimnt issue description models.
        /// </summary>
        /// <value>
        /// The impedimnt issue description models.
        /// </value>
        public List<IssueDescriptionModel> ImpedimntIssueDescriptionModels { get; set; }
    }
}