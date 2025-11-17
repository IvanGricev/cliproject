using CLI.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Threading;
using System.Threading.Tasks; // <-- Added

namespace CLI.Shell.Commands
{
    // Changed to AsyncCommand
    public class ModulesCommand : AsyncCommand
    {
        private readonly ShellState _shellState;

        public ModulesCommand(ShellState shellState)
        {
            _shellState = shellState;
        }

        // Changed to ExecuteAsync, returns Task<int>
        public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var modulesTable = new Table().Border(TableBorder.Rounded).Expand();
            modulesTable.Title = new TableTitle("Available Modules");
            modulesTable.AddColumn("Name");
            modulesTable.AddColumn("Description");

            if (_shellState.AvailableModules.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No modules found.[/]");
            }
            else
            {
                foreach (var mod in _shellState.AvailableModules)
                {
                    modulesTable.AddRow($"[bold cyan]{mod.Name}[/]", mod.Description);
                }
                AnsiConsole.Write(modulesTable);
            }
            return Task.FromResult(0);
        }
    }
}