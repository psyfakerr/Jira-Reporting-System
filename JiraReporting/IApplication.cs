using System.Threading.Tasks;

namespace JiraReporting
{
    /// <summary>
    /// Application interface
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <returns></returns>
        Task RunAsync();
    }
}
