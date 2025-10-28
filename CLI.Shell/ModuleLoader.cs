using CLI.Core;
using System.Reflection;

namespace CLI.Shell
{
    public class ModuleLoader
    {
        public List<ICommandModule> LoadModules(string path)
        {
            var modules = new List<ICommandModule>();
            
            // Ensure the modules directory exists
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Modules directory not found.");
                return modules;
            }

            // Get all .dll files in the directory
            var dllFiles = Directory.GetFiles(path, "*.dll");

            foreach (var file in dllFiles)
            {
                try
                {
                    // Load the assembly
                    var assembly = Assembly.LoadFrom(file);
                    
                    // Find all types in the assembly that implement ICommandModule
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(ICommandModule).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var type in moduleTypes)
                    {
                        // Create an instance of the module and add it to our list
                        var module = (ICommandModule)Activator.CreateInstance(type);
                        if (module != null)
                        {
                            modules.Add(module);
                            Console.WriteLine($"Loaded module: {module.Name}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load module from {file}: {ex.Message}");
                }
            }
            return modules;
        }
    }
}