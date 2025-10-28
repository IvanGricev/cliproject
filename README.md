CLI Project

A modular, plugin-based console application written in C#. This project acts as a core "shell" that can load external modules (plugins) at runtime, allowing new features to be added without modifying the main application.

This project is built using .NET and follows a clean architecture pattern to decouple the core shell from its modules.

Table of Contents

Features

Project Structure

Getting Started

Prerequisites

1. Restore Dependencies

2. Build the Solution

3. Run the Shell

How to Create a New Module

Step 1: Create the Project

Step 2: Add the Core Reference

Step 3: Implement the Interface

Step 4: Set up the Post-Build Event

Step 5: Build & Run

Module Design Rules

Features

Plugin Architecture: The shell loads all module .dll files from a /modules folder at startup.

REPL Interface: A standard Read-Evaluate-Print Loop for command entry.

State Management: The shell manages the state, allowing the user to "enter" a module and use its specific commands.

Robust Command Parsing: Modules use the Spectre.Console.Cli library to define and parse their own complex commands, arguments, and options.

Data Persistence: Modules are responsible for their own data persistence (e.g., the Notes module saves its notes to a JSON file).

Project Structure

The solution is divided into several key projects:

CLISolution.sln: The main Visual Studio Solution that contains all projects.

CLI.Core/: A Class Library. This is the most important project. It contains the ICommandModule interface, which is the "contract" that all modules must implement. Both the Shell and all Modules reference this project.

CLI.Shell/: The main Console App. This is the executable. It contains:

The REPL (Program.cs).

The ModuleLoader class (using Reflection).

The main "shell" commands (help, modules, enter_module, exit).

CLI.Module.Notes/: A Class Library. This is an example of a module that implements ICommandModule. It shows how to use Spectre.Console.Cli for internal commands and how to manage persistent data.

Getting Started (Setup & Run)

Follow these steps to build and run the application.

Prerequisites

.NET 8.0 SDK (or newer)

A terminal (like PowerShell, cmd, or bash)

VS Code (or your preferred editor)

1. Restore Dependencies

First, restore all the NuGet packages required by the solution:

dotnet restore


2. Build the Solution

[!WARNING]
This step is critical. Building the solution will not only compile the code but also run the Post-Build Events that copy the module .dll files into the correct modules folder for the shell to find.

dotnet build


3. Run the Shell

You only run the CLI.Shell project.

dotnet run --project CLI.Shell/CLI.Shell.csproj


If everything is set up correctly, you will see a "Loaded module: Notes" message, and you'll be at the main > prompt.

How to Create a New Module

This guide will walk you through creating a new module named CLI.Module.MyNewModule.

Step 1: Create the Project

In your main cliproject folder, run the following commands in your terminal:

# 1. Create a new Class Library project
dotnet new classlib -n CLI.Module.MyNewModule

# 2. Add the new project to your main solution
dotnet sln add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj


Step 2: Add the Core Reference

Your new module must reference CLI.Core to get the ICommandModule interface.

dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj reference CLI.Core/CLI.Core.csproj


Step 3: Implement the ICommandModule Interface

Rename Class1.cs in your new project to MyNewModule.cs and implement the interface.

We strongly recommend using Spectre.Console.Cli to manage your module's internal commands, just as the Notes module does.

Here is a simple template to get you started:

using CLI.Core;
using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using System.Threading; // Added for CancellationToken

namespace CLI.Module.MyNewModule
{
    public class MyNewModule : ICommandModule
    {
        public string Name => "MyModule";
        public string Description => "A description of what my module does.";

        private readonly ICommandApp _app;

        public MyNewModule()
        {
            // 1. Set up the Dependency Injection to manage state (if needed)
            var services = new ServiceCollection();
            // services.AddSingleton(new MyModuleState()); // Example

            // 2. Create your command app
            _app = new CommandApp(new TypeRegistrar(services));
            
            // 3. Configure your commands
            _app.Configure(config =>
            {
                config.AddCommand<MyCommand>("my-command")
                    .WithDescription("Does a cool thing.");
                
                // Add more commands here
            });
        }

        public void ShowHelp()
        {
            // The shell passes "help" to this app
            _app.Run(new[] { "--help" });
        }

        public void ProcessCommand(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            try
            {
                _app.Run(args);
            }
            catch (Exception ex)
            {
                // Spectre automatically prints nice errors, 
                // but you can catch them if needed.
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // --- Define your commands and settings classes ---
    
    public class MyCommandSettings : CommandSettings
    {
        [CommandArgument(0, "[NAME]")]
        public string Name { get; set; }
    }

    public class MyCommand : Command<MyCommandSettings>
    {
        public override int Execute(CommandContext context, MyCommandSettings settings, CancellationToken cancellationToken)
        {
            string name = settings.Name ?? "World";
            Console.WriteLine($"Hello, {name}!");
            return 0;
        }
    }
}


For this to work, you must also add the Spectre.Console.Cli packages to your new project:

dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj package Spectre.Console.Cli
dotnet add CLI.Module.MyNewModule/CLI.Module.MyNewModule.csproj package Spectre.Console.Cli.Extensions.DependencyInjection


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


Module Design Rules

Modules are Self-Contained: A module should manage its own state and persistence. It should never try to directly access another module.

Use Spectre.Console.Cli: All modules should use this library for internal command parsing. Do not use input.Split() logic.

Handle Your Own Persistence: If your module needs to save data, save it to a file (e.g., in %APPDATA% or a local .json file). The Notes module is the reference for this.

Do Not Handle exit: The main CLI.Shell is responsible for handling the exit and module_exit commands. Your module's ShowHelp() text should not include these commands.