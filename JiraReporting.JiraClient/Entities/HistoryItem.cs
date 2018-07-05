using Newtonsoft.Json;

namespace JiraReporting.JiraClient.Entities
{
    /// <summary>
    /// History item
    /// </summary>
    [JsonObject]
    public class HistoryItem
    {
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [JsonProperty("field")]
        public string Field { get; set; }
    }
}
