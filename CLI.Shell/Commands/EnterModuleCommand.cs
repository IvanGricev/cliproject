using CLI.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Linq;
using System.Threading; // <-- Make sure this is present
using System.Threading.Tasks;

namespace CLI.Shell.Commands
{
    public class EnterModuleSettings : CommandSettings
    {
        [CommandArgument(0, "<MODULE_NAME>")]
        [Description("The name of the module to enter.")]
        public string ModuleName { get; set; }
    }

    public class EnterModuleCommand : AsyncCommand<EnterModuleSettings>
    {
        private readonly ShellState _shellState;

        public EnterModuleCommand(ShellState shellState)
        {
            _shellState = shellState;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, EnterModuleSettings settings, CancellationToken cancellationToken)
        {
            var moduleToEnter = _shellState.AvailableModules
                .FirstOrDefault(m => m.Name.Equals(settings.ModuleName, StringComparison.OrdinalIgnoreCase));

            if (moduleToEnter != null)
            {
                _shellState.CurrentModule = moduleToEnter;
                AnsiConsole.MarkupLine($"Entering module: [bold cyan]{_shellState.CurrentModule.Name}[/]. Type 'module_exit' to return.");
                
                // We call the async version of ShowHelp
                await _shellState.CurrentModule.ShowHelpAsync();
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Module not found.[/]");
            }
            return 0;
        }
    }
}