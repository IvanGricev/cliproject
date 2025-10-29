using System;
using System.IO;
using System.Text.Json;

namespace CLI.Core
{
    /// <summary>
    /// A generic service for serializing and deserializing data to JSON files
    /// in a central 'ModulesData' folder in the project root.
    /// </summary>
    public class JsonDataService
    {
        private readonly string _baseDataPath;

        public JsonDataService()
        {
            // 1. Get the directory where the .exe is running
            // (e.g., .../cliproject/CLI.Shell/bin/Debug/net9.0/)
            string exeDir = AppContext.BaseDirectory;

            // 2. Find the solution root (cliproject) by searching upwards for the .sln file
            DirectoryInfo currentDir = new DirectoryInfo(exeDir);
            string solutionRoot = null;
            while (currentDir != null)
            {
                // Check if a .sln file exists here
                if (currentDir.GetFiles("*.sln").Length > 0)
                {
                    solutionRoot = currentDir.FullName;
                    break;
                }
                
                // If not, go up one level
                currentDir = currentDir.Parent;
            }

            // 3. If we can't find it (e.g., in a weird test environment),
            // just default to the exe directory
            if (solutionRoot == null)
            {
                solutionRoot = exeDir;
                Console.WriteLine("Warning: .sln file not found. Saving data in exe directory.");
            }

            // 4. Define the path for our central data folder
            _baseDataPath = Path.Combine(solutionRoot, "ModulesData");

            // 5. Create the directory if it doesn't already exist
            if (!Directory.Exists(_baseDataPath))
            {
                try
                {
                    Directory.CreateDirectory(_baseDataPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FATAL: Could not create ModulesData directory at {_baseDataPath}. Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the full, absolute path for a given data file.
        /// </summary>
        /// <param name="fileName">The simple file name (e.g., "notes.json")</param>
        private string GetFullFilePath(string fileName)
        {
            return Path.Combine(_baseDataPath, fileName);
        }

        /// <summary>
        /// Saves data to a JSON file inside the ModulesData folder.
        /// </summary>
        /// <typeparam name="T">The type of data to save.</typeparam>
        /// <param name="fileName">The name of the file (e.g., "notes.json").</param>
        /// <param name="data">The data object to serialize.</param>
        public void SaveData<T>(string fileName, T data)
        {
            string filePath = GetFullFilePath(fileName);
            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads data from a JSON file inside the ModulesData folder.
        /// </summary>
        /// <typeparam name="T">The type of data to load.</typeparam>
        /// <param name="fileName">The name of the file (e.g., "notes.json").</param>
        /// <returns>The deserialized data, or default(T) if not found or on error.</returns>
        public T LoadData<T>(string fileName)
        {
            string filePath = GetFullFilePath(fileName);

            if (!File.Exists(filePath))
            {
                // Return default (e.g., null for objects) if the file doesn't exist
                return default(T);
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from {filePath}: {ex.Message}");
                // Return default on error
                return default(T);
            }
        }
    }
}

