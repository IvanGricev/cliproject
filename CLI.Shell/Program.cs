using CLI.Core;
using CLI.Shell;

var loader = new ModuleLoader();


string modulesPath = Path.Combine(AppContext.BaseDirectory, "modules");

List<ICommandModule> availableModules = loader.LoadModules(modulesPath);

ICommandModule currentModule = null; // null means we are in the main shell

Console.WriteLine("Welcome to CLI. Type 'help' for commands.");

// The REPL (Read-Evaluate-Print Loop)
while (true)
{
    string prompt = (currentModule == null) ? "> " : $"{currentModule.Name}> ";
    Console.Write(prompt);

    string input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    // --- Command Evaluation ---
    if (currentModule == null)
    {
        // --- We are in the MAIN SHELL ---
        if (input == "help")
        {
            Console.WriteLine("--- Main Help ---");
            Console.WriteLine("help                - Shows this help");
            Console.WriteLine("modules             - Lists all available modules");
            Console.WriteLine("enter_module <name> - Enters a specific module");
            Console.WriteLine("exit                - Exits the CLI");
        }
        else if (input == "modules")
        {
            Console.WriteLine("--- Available Modules ---");
            if (availableModules.Count == 0)
            {
                Console.WriteLine("No modules found.");
            }
            foreach (var mod in availableModules)
            {
                Console.WriteLine($"{mod.Name} - {mod.Description}");
            }
        }
        else if (input.StartsWith("enter_module "))
        {
            string moduleName = input.Substring(13);
            var moduleToEnter = availableModules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));

            if (moduleToEnter != null)
            {
                currentModule = moduleToEnter;
                Console.WriteLine($"Entering module: {currentModule.Name}. Type 'exit' to return.");
                currentModule.ShowHelp(); // Show module-specific help
            }
            else
            {
                Console.WriteLine("Module not found.");
            }
        }
        else if (input == "exit")
        {
            break; // Exit the while loop and end the program
        }
        else
        {
            Console.WriteLine("Unknown command. Type 'help'.");
        }
    }
    else
    {
        // --- We are INSIDE A MODULE ---
        if (input == "exit" || input == "module_exit")
        {
            Console.WriteLine($"Exiting {currentModule.Name} module.");
            currentModule = null; // Go back to the main shell
        }
        else
        {
            // Pass the command to the active module to handle
            currentModule.ProcessCommand(input);
        }
    }
}