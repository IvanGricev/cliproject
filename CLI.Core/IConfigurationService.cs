using System.Text.Json.Nodes;

namespace CLI.Core
{
    /// <summary>
    /// Provides access to configuration values from config.json.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets a configuration value by its key.
        /// </summary>
        /// <param name="key">The key (e.g., "GitHub:ApiKey").</param>
        /// <returns>The value as a string, or null if not found.</returns>
        string GetValue(string key);
    }
}

