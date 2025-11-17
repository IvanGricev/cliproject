using Spectre.Console.Cli;
using System.Threading;
using System.Threading.Tasks; // <-- Added

namespace CLI.Shell.Commands
{
    // Changed to AsyncCommand
    public class ExitCommand : AsyncCommand
    {
        private readonly ShellState _shellState;

        public ExitCommand(ShellState shellState)
        {
            _shellState = shellState;
        }

        // Changed to ExecuteAsync, returns Task<int>
        public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _shellState.IsRunning = false;
            return Task.FromResult(0);
        }
    }
}