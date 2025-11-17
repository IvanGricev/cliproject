using CLI.Core;
using CLI.Shell;
using CLI.Shell.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

Console.Title = "CLI Shell";

var loader = new ModuleLoader();
string modulesPath = Path.Combine(AppContext.BaseDirectory, "modules");
ModuleLoadResult loadResult = loader.LoadModules(modulesPath);

var shellState = new ShellState(loadResult.LoadedModules);

var services = new ServiceCollection();
services.AddSingleton(shellState);
var registrar = new SpectreTypeRegistrar(services);
var shellApp = new CommandApp(registrar);

shellApp.Configure(config =>
{
    config.AddCommand<ShellHelpCommand>("help").WithDescription("Shows this help");
    config.AddCommand<ModulesCommand>("modules").WithDescription("Lists all available modules");
    config.AddCommand<EnterModuleCommand>("enter_module").WithDescription("Enters a specific module");
    config.AddCommand<ExitCommand>("exit").WithDescription("Exits the CLI");
    
    config.SetApplicationName("");
    config.ValidateExamples();
});

AnsiConsole.MarkupLine("[bold blue]Welcome to CLI.[/] Type 'help' for commands.");
AnsiConsole.MarkupLine($"[green]Successfully loaded {loadResult.LoadedModules.Count} module(s).[/]");

if (loadResult.FailedModules.Count > 0)
{
    var table = new Table().Border(TableBorder.Rounded).Expand();
    table.AddColumn("Module File");
    table.AddColumn("Error");
    foreach (var failed in loadResult.FailedModules)
    {
        table.AddRow($"[yellow]{failed.FileName}[/]", $"[red]{failed.Error.EscapeMarkup()}[/]");
    }
    AnsiConsole.Write(table);
}

while (shellState.IsRunning)
{
    var prompt = shellState.CurrentModule == null 
        ? new Markup("[yellow]> [/]") 
        : new Markup($"[cyan]{shellState.CurrentModule.Name}> [/]");
    AnsiConsole.Write(prompt);

    string input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var inputArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    if (inputArgs.Length == 0) continue;

    if (shellState.CurrentModule == null)
    {
        try
        {
            await shellApp.RunAsync(inputArgs);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
        }
    }
    else
    {
        if (inputArgs[0].ToLower() == "exit" || inputArgs[0].ToLower() == "module_exit")
        {
            AnsiConsole.MarkupLine($"Exiting [cyan]{shellState.CurrentModule.Name}[/] module.");
            shellState.CurrentModule = null; 
        }
        else if (inputArgs[0].ToLower() == "help")
        {
            await shellState.CurrentModule.ShowHelpAsync();
        }
        else
        {
            await shellState.CurrentModule.ProcessCommandAsync(input);
        }
    }
}