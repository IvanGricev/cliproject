using CLI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CLI.Shell
{
    /// <summary>
    /// Contains the result of a module load operation.
    /// </summary>
    public class ModuleLoadResult
    {
        public List<ICommandModule> LoadedModules { get; } = new List<ICommandModule>();
        public List<(string FileName, string Error)> FailedModules { get; } = new List<(string, string)>();
    }

    public class ModuleLoader
    {
        public ModuleLoadResult LoadModules(string path)
        {
            var result = new ModuleLoadResult();

            if (!Directory.Exists(path))
            {
                return result; // Return empty result, folder doesn't exist
            }

            var dllFiles = Directory.GetFiles(path, "*.dll");

            foreach (var file in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(ICommandModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in moduleTypes)
                    {
                        var module = (ICommandModule)Activator.CreateInstance(type);
                        if (module != null)
                        {
                            result.LoadedModules.Add(module);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Catch the error and add it to the failed list
                    result.FailedModules.Add((Path.GetFileName(file), ex.Message));
                }
            }
            return result;
        }
    }
}

