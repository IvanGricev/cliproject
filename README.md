CLI Project

A modular, plugin-based console application written in C#. This project acts as a core "shell" that can load external modules (plugins) at runtime, allowing new features to be added without modifying the main application.

The project is built on .NET and follows clean architecture principles to decouple the core shell from its modules.

** Table of Contents

** Features

** Project Structure

** Getting Started

** How to Create a New Module

** Module Design Rules

** Features

Plugin Architecture: The shell loads all module .dll files from a /modules folder at startup using Reflection.

REPL Interface: A standard Read-Evaluate-Print Loop for command entry.

State Management: The shell manages the state, allowing the user to "enter" a module and use its specific commands.

Robust Command Parsing: Modules use the Spectre.Console.Cli library to define and parse their own complex commands, arguments, and options.

Centralized Data Storage: A JsonDataService in CLI.Core provides easy-to-use methods (LoadData, SaveData) for all modules to persist their data in a central ModulesData folder.

Shared DI Registrar: CLI.Core provides a common SpectreTypeRegistrar to simplify setting up Dependency Injection for commands.

** Project Structure

The solution is divided into several key projects:

CLISolution.sln
├── 📁 CLI.Core/
│   ├── ICommandModule.cs       (The "contract" for all modules)
│   ├── SpectreTypeRegistrar.cs (Shared DI helper for Spectre)
│   └── JsonDataService.cs      (Shared helper for JSON Load/Save)
│
├── 📁 CLI.Shell/
│   ├── Program.cs              (The main REPL loop)
│   └── ModuleLoader.cs         (Finds and loads module .dlls)
│
└── 📁 CLI.Module.Notes/
    ├── NotesModule.cs          (Example module implementation)
    └── Commands/               (Subfolder for Spectre command classes)


CLI.Core: A Class Library. This is the shared core. Both the Shell and all Modules reference this project.

CLI.Shell: The main Console App. This is the executable REPL and module host.

CLI.Module.Notes: A Class Library. This is an example module that demonstrates data persistence and command parsing.

** Getting Started

Follow these steps to build and run the application.

Prerequisites

.NET 8.0 SDK (or newer)

A terminal (like PowerShell, cmd, or bash)

1. Restore Dependencies

First, restore all the NuGet packages required by the solution:

dotnet restore


2. Build the Solution

Warning
This step is critical. Building the solution will not only compile the code but also run the Post-Build Events that copy the module .dll files into the correct modules folder for the shell to find.

dotnet build


3. Run the Shell

You only run the CLI.Shell project.

dotnet run --project CLI.Shell/CLI.Shell.csproj


If everything is set up correctly, you will see a "Loaded module: Notes" message, and you'll be at the main > prompt.

** How to Create a New Module

This guide will walk you through creating a new module named CLI.Module.MyNewModule.

Step 1: Create the Project

In your main cliproject folder, run the following commands in your terminal:

# 1. Create a new Class Library project
dotnet new classlib -n CLI.Module.MyNewModule

# 2. Add the new project to your main solution
dotnet sln add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj


Step 2: Add Required References

Your new module must reference CLI.Core (to get the interface and helpers) and Spectre.Console.Cli (to define commands).

# 1. Add the Core project reference
dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj reference CLI.Core/CLI.Core.csproj

# 2. Add the Spectre.Console.Cli package
dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj package Spectre.Console.Cli


Step 3: Implement the ICommandModule Interface

Rename Class1.cs in your new project to MyNewModule.cs and implement the interface.

Here is a simple template to get you started. It demonstrates:

Using the shared SpectreTypeRegistrar from CLI.Core.

Using the shared JsonDataService from CLI.Core.

using CLI.Core; // <-- Imports ICommandModule, JsonDataService, SpectreTypeRegistrar
using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace CLI.Module.MyNewModule
{
    public class MyNewModule : ICommandModule
    {
        public string Name => "MyModule";
        public string Description => "A description of what my module does.";

        private readonly ICommandApp _app;
        private readonly JsonDataService _dataService;
        
        // This module's own data, loaded from a file
        private MyModuleSettings _settings;
        private const string _settingsFile = "mymodule.json";

        public MyNewModule()
        {
            // 1. Set up data persistence
            _dataService = new JsonDataService();
            _settings = _dataService.LoadData<MyModuleSettings>(_settingsFile) ?? new MyModuleSettings();

            // 2. Set up Dependency Injection
            var services = new ServiceCollection();
            services.AddSingleton(_settings); // Inject our settings object

            // 3. Create the command app using the shared registrar
            _app = new CommandApp(new SpectreTypeRegistrar(services));
            
            // 4. Configure your commands
            _app.Configure(config =>
            {
                config.AddCommand<MyCommand>("my-command")
                    .WithDescription("Does a cool thing.");
                
                // Add more commands here
            });
        }

        public void ShowHelp()
        {
            _app.Run(new[] { "--help" });
        }

        public void ProcessCommand(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            try
            {
                int result = _app.Run(args);
                
                // If a state-changing command succeeded, save the data
                if (result == 0 && args.Length > 0 && args[0] == "my-command")
                {
                    _dataService.SaveData(_settingsFile, _settings);
                }
            }
            catch (Exception ex)
            {
                // Spectre automatically prints nice errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // --- Define your data, command, and settings classes ---
    
    // Example data class for saving/loading
    public class MyModuleSettings
    {
        public int ExampleSetting { get; set; } = 0;
    }

    // Example command
    public class MyCommand : Command
    {
        private readonly MyModuleSettings _settings;
        public MyCommand(MyModuleSettings settings)
        {
            _settings = settings; // Get settings via DI
        }

        public override int Execute(CommandContext context, CancellationToken cancellationToken)
        {
            _settings.ExampleSetting++;
            Console.WriteLine($"Command executed! Setting is now: {_settings.ExampleSetting}");
            return 0;
        }
    }
}


Step 4: Set up the Post-Build Event

This is the "magic" step. You must edit your new .csproj file (CLI.Module.MyNewModule.csproj) to automatically copy the compiled .dll to the Shell's modules folder.

Open the .csproj file and add this Target block at the bottom, right before the closing </Project> tag:

  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="mkdir &quot;$(SolutionDir)CLI.Shell\bin\$(Configuration)\$(TargetFramework)\modules&quot; 2&gt;nul || (exit 0)&#xD;&#xA;copy &quot;$(TargetDir)$(ProjectName).dll&quot; &quot;$(SolutionDir)CLI.Shell\bin\$(Configuration)\$(TargetFramework)\modules\$(ProjectName).dll&quot;" />
  </Target>


Step 5: Build & Run

That's it! Now, just build the solution. The Post-Build script will run automatically.

dotnet build


Then, run the shell. You should see your new module loaded on startup.

dotnet run --project CLI.Shell/CLI.Shell.csproj


Output:

Loaded module: Notes
Loaded module: MyModule  <-- Success!
Welcome to CLI. Type 'help' for commands.
>


** Module Design Rules

Modules are Self-Contained: A module should manage its own state and persistence. It should never try to directly access another module.

Use Core Services:

All modules must use Spectre.Console.Cli for internal command parsing.

All modules must use the SpectreTypeRegistrar from CLI.Core for DI.

All modules should use the JsonDataService from CLI.Core for data persistence.

Do Not Handle exit: The main CLI.Shell is responsible for handling the exit and module_exit commands. Your module's ShowHelp() text should not include these commands.