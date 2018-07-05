using System.Collections.Generic;
using Newtonsoft.Json;

namespace JiraReporting.JiraClient.Entities
{
    /// <summary>
    /// ChangeLog
    /// </summary>
    [JsonObject]
    public class Changelog
    {
        /// <summary>
        /// Gets or sets the historiess.
        /// </summary>
        /// <value>
        /// The historiess.
        /// </value>
        [JsonProperty("histories")]
        public List<History> Historiess { get; set; }
    }
}
