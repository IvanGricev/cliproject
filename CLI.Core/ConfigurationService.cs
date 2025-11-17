using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CLI.Core
{
    /// <summary>
    /// Implementation of IConfigurationService that loads config.json from the ModulesData folder.
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly JsonNode? _configRoot;

        public ConfigurationService(JsonDataService dataService)
        {
            try
            {
                _configRoot = dataService.LoadDataAsync<JsonNode>("config.json").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Could not load config.json. {ex.Message}");
                Console.ResetColor();
            }
        }

        public string? GetValue(string key)
        {
            if (_configRoot == null)
            {
                return null;
            }

            // Handle nested keys separated by colons, e.g., "GitHub:ApiKey"
            var parts = key.Split(':');
            JsonNode? currentNode = _configRoot; // Make nullable

            foreach (var part in parts)
            {
                if (currentNode is JsonObject obj && obj.ContainsKey(part))
                {
                    currentNode = obj[part];
                }
                else
                {
                    return null; // Key not found
                }
            }

            // Return the value as a string
            return currentNode?.AsValue()?.ToString();
        }
    }
}