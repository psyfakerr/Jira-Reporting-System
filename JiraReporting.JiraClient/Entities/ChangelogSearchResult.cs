using Newtonsoft.Json;

namespace JiraReporting.JiraClient.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PageableResult" />
    [JsonObject]
    public class ChangelogSearchResult : PageableResult
    {
        /// <summary>
        ///     Results
        /// </summary>
        [JsonProperty("changelog")]
        public Changelog Changelog { get; set; }
    }
}
