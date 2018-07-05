using System.Collections.Generic;

namespace JiraReporting.Models
{
    /// <summary>
    /// Work team model
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the name of the project agile board.
        /// </summary>
        /// <value>
        /// The name of the project agile board.
        /// </value>
        public string ProjectAgileBoardName { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        /// <value>
        /// The name of the team.
        /// </value>
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the team members names.
        /// </summary>
        /// <value>
        /// The team members names.
        /// </value>
        public IEnumerable<string> TeamMembersNames { get; set; }
    }
}