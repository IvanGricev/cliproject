CLI Project

A modular, plugin-based console application written in C#. This project acts as a core "shell" that can load external modules (plugins) at runtime, allowing new features to be added without modifying the main application.

The project is built on .NET and follows clean architecture principles to decouple the core shell from its modules.

Table of Contents

Features

Project Structure

Getting Started

How to Create a New Module

Module Design Rules

Features

Plugin Architecture: The shell loads all module .dll files from a /modules folder at startup using Reflection and reports any load failures.

Asynchronous by Default: The entire application, from the shell's REPL to the module commands, is built on async/await to handle I/O without blocking.

Rich REPL Interface: A standard Read-Evaluate-Print Loop for command entry, enhanced with a Spectre.Console UI for colors and tables.

Robust Command Parsing: The shell and all modules use the Spectre.Console.Cli library to define and parse their own complex commands, arguments, and options.

Simplified Module Creation: A BaseModule abstract class in CLI.Core handles 90% of the boilerplate for creating new modules (DI, Spectre setup, etc.).

Centralized Services: CLI.Core provides shared, reusable services for:

JsonDataService: Easy-to-use async methods (LoadDataAsync, SaveDataAsync) for JSON persistence.

IConfigurationService: Loads shared configuration (e.g., API keys) from a central config.json.

SpectreTypeRegistrar: A shared DI helper for Spectre.Console.Cli.

Project Structure

The solution is divided into several key projects:

CLISolution.sln
├── 📁 CLI.Core/
│   ├── ICommandModule.cs         (The async "contract" for all modules)
│   ├── BaseModule.cs           (Abstract class to simplify module creation)
│   ├── IConfigurationService.cs  (Interface for a shared config service)
│   ├── ConfigurationService.cs   (Implementation for config.json)
│   ├── SpectreTypeRegistrar.cs   (Shared DI helper for Spectre)
│   └── JsonDataService.cs        (Shared helper for async JSON Load/Save)
│
├── 📁 CLI.Shell/
│   ├── Program.cs                (The main async REPL loop)
│   ├── ModuleLoader.cs           (Finds and loads module .dlls from memory)
│   └── Commands/                 (Folder for the Shell's own internal commands)
│       ├── ShellState.cs
│       ├── EnterModuleCommand.cs
│       └── ...
│
└── 📁 CLI.Module.Notes/
    ├── NotesModule.cs            (Example module inheriting from BaseModule)
    └── Commands/                 (Subfolder for Spectre async command classes)
        ├── AddNoteCommand.cs
        └── ...


CLI.Core: A Class Library. This is the shared core. Both the Shell and all Modules reference this project.

CLI.Shell: The main Console App. This is the executable REPL and module host.

CLI.Module.Notes: A Class Library. This is an example module that demonstrates persistence and command parsing by inheriting from BaseModule.

Getting Started

Follow these steps to build and run the application.

Prerequisites

.NET 8.0 SDK (or newer)

A terminal (like PowerShell, cmd, or bash)

1. Restore Dependencies

First, restore all the NuGet packages required by the solution:

dotnet restore


2. Build the Solution

[!WARNING]
This step is critical. Building the solution will not only compile the code but also run the Post-Build target that copies the module .dll files into the correct modules folder for the shell to find.

dotnet build


3. Run the Shell

You only run the CLI.Shell project.

dotnet run --project CLI.Shell/CLI.Shell.csproj


If everything is set up correctly, you will see a "Successfully loaded 1 module(s)." message, and you'll be at the main > prompt.

How to Create a New Module

This guide will walk you through creating a new module named CLI.Module.MyNewModule.

Step 1: Create the Project

In your main cliproject folder, run the following commands in your terminal:

# 1. Create a new Class Library project
dotnet new classlib -n CLI.Module.MyNewModule

# 2. Add the new project to your main solution
dotnet sln add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj


Step 2: Add Required References

Your new module must reference CLI.Core (to get BaseModule and services) and Spectre.Console.Cli (to define commands).

# 1. Add the Core project reference
dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj reference CLI.Core/CLI.Core.csproj

# 2. Add the Spectre.Console.Cli package
dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj package Spectre.Console.Cli


Step 3: Inherit from BaseModule

Instead of implementing ICommandModule by hand, just inherit from BaseModule.

Rename Class1.cs in your new project to MyNewModule.cs and use this template. It demonstrates inheriting from BaseModule and using all the shared services.

using CLI.Core; // <-- Imports BaseModule, JsonDataService, etc.
using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace CLI.Module.MyNewModule
{
    // 1. Inherit from BaseModule
    public class MyNewModule : BaseModule
    {
        public override string Name => "MyModule";
        public override string Description => "A description of what my module does.";

        private MyModuleSettings _settings;
        private const string _settingsFile = "mymodule.json";

        public MyNewModule() : base() { } // Base constructor handles setup

        // 2. Load and register your module's services
        protected override void ConfigureServices(IServiceCollection services)
        {
            // The 'DataService' is inherited from BaseModule
            _settings = DataService.LoadDataAsync<MyModuleSettings>(_settingsFile).GetAwaiter().GetResult()
                        ?? new MyModuleSettings();
            
            services.AddSingleton(_settings); // Inject our settings object
        }

        // 3. Configure your module's commands
        protected override void ConfigureCommands(IConfigurator config)
        {
            config.AddCommand<MyCommand>("my-command")
                  .WithDescription("Does a cool thing.");
            
            // Add more commands here
        }

        // 4. (Optional) Hook into successful commands to save data
        protected override async Task OnCommandExecutedAsync(string commandName, string[] args)
        {
            if (commandName == "my-command")
            {
                await DataService.SaveDataAsync(_settingsFile, _settings);
            }
        }
    }

    // --- Define your data, command, and settings classes ---
    
    // Example data class for saving/loading
    public class MyModuleSettings
    {
        public int ExampleSetting { get; set; } = 0;
    }

    // Example command (must be AsyncCommand)
    public class MyCommand : AsyncCommand
    {
        private readonly MyModuleSettings _settings;
        public MyCommand(MyModuleSettings settings)
        {
            _settings = settings; // Get settings via DI
        }

        public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _settings.ExampleSetting++;
            Console.WriteLine($"Command executed! Setting is now: {_settings.ExampleSetting}");
            return Task.FromResult(0);
        }
    }
}


Step 4: Set up the Post-Build Event

This is the "magic" step. Edit your new .csproj file (CLI.Module.MyNewModule.csproj) to automatically copy the compiled .dll to the Shell's modules folder.

Open the .csproj file and add this robust Target block at the bottom, right before the closing </Project> tag:

  <!-- This is the robust, platform-agnostic way to copy the DLL -->
  <Target Name="CopyPluginToShell" AfterTargets="Build">
    <PropertyGroup>
      <!-- Define the destination folder -->
      <ShellModulesDir>$(SolutionDir)CLI.Shell\bin\$(Configuration)\$(TargetFramework)\modules\</ShellModulesDir>
    </PropertyGroup>

    <!-- Create the 'modules' directory if it doesn't exist -->
    <MakeDir Directories="$(ShellModulesDir)" Condition="!Exists('$(ShellModulesDir)')" />

    <!-- Copy the DLL -->
    <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(ShellModulesDir)" />
  </Target>


Step 5: Build & Run

That's it! Now, just build the solution. The Post-Build target will run automatically.

dotnet build


Then, run the shell. You should see your new module loaded on startup.

dotnet run --project CLI.Shell/CLI.Shell.csproj


Output:

[bold blue]Welcome to CLI.[/] Type 'help' for commands.
[green]Successfully loaded 2 module(s).[/]  <-- Success!
>


Module Design Rules

Modules are Self-Contained: A module should manage its own state and persistence. It should never try to directly access another module.

Inherit from BaseModule: All modules must inherit from BaseModule in CLI.Core.

Use Core Services: All modules should use the inherited DataService and ConfigService for I/O.

Design for async: All commands must inherit from AsyncCommand or AsyncCommand<T> and be non-blocking.

Do Not Handle exit: The main CLI.Shell is responsible for handling the exit and module_exit commands. Your module's ShowHelp() text should not include these commands.