using Spectre.Console;
using Spectre.Console.Cli;
using System.Threading;
using System.Threading.Tasks; // <-- Added

namespace CLI.Shell.Commands
{
    /// <summary>
    /// Overrides the default 'help' command to show our custom table.
    /// </summary>
    // Changed to AsyncCommand
    public class ShellHelpCommand : AsyncCommand
    {
        // Changed to ExecuteAsync, returns Task<int>
        public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var helpTable = new Table().Border(TableBorder.None).Expand().HideHeaders();
            helpTable.AddColumn("Command").AddColumn("Description");
            helpTable.AddRow("[blue]help[/]", "Shows this help");
            helpTable.AddRow("[blue]modules[/]", "Lists all available modules");
            helpTable.AddRow("[blue]enter_module <name>[/]", "Enters a specific module");
            helpTable.AddRow("[blue]exit[/]", "Exits the CLI");
            AnsiConsole.Write(helpTable);
            return Task.FromResult(0);
        }
    }
}