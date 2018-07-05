namespace JiraReporting.Models
{
    /// <summary>
    /// Issue description model
    /// </summary>
    public class IssueDescriptionModel
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent task.
        /// </summary>
        /// <value>
        /// The name of the parent task.
        /// </value>
        public string ParentTaskName { get; set; }

        /// <summary>
        /// Gets or sets the issue status.
        /// </summary>
        /// <value>
        /// The issue status.
        /// </value>
        public string IssueStatus { get; set; }

        /// <summary>
        /// Gets or sets the issue link.
        /// </summary>
        /// <value>
        /// The issue link.
        /// </value>
        public string IssueLink { get; set; }

        /// <summary>
        /// Gets or sets the issue key.
        /// </summary>
        /// <value>
        /// The issue key.
        /// </value>
        public string IssueKey { get; set; }
    }
}