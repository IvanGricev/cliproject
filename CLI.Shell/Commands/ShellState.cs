using CLI.Core;
using System.Collections.Generic;

namespace CLI.Shell.Commands
{
    /// <summary>
    /// A state object to be passed via DI to shell commands.
    /// </summary>
    public class ShellState
    {
        public List<ICommandModule> AvailableModules { get; }
        public ICommandModule CurrentModule { get; set; } = null;
        public bool IsRunning { get; set; } = true;

        public ShellState(List<ICommandModule> availableModules)
        {
            AvailableModules = availableModules;
        }
    }
}