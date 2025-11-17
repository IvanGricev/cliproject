using Spectre.Console;
using Spectre.Console.Cli;
using System.Threading;

namespace CLI.Shell.Commands
{
    /// <summary>
    /// Overrides the default 'help' command to show our custom table.
    /// </summary>
    public class ShellHelpCommand : Command
    {
        public override int Execute(CommandContext context, CancellationToken cancellationToken)
        {
            var helpTable = new Table().Border(TableBorder.None).Expand().NoHeaders();
            helpTable.AddColumn("Command").AddColumn("Description");
            helpTable.AddRow("[blue]help[/]", "Shows this help");
            helpTable.AddRow("[blue]modules[/]", "Lists all available modules");
            helpTable.AddRow("[blue]enter_module <name>[/]", "Enters a specific module");
            helpTable.AddRow("[blue]exit[/]", "Exits the CLI");
            AnsiConsole.Write(helpTable);
            return 0;
        }
    }
}