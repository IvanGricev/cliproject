using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks; // <-- Added

namespace CLI.Module.Notes.Commands
{
    // Changed to AsyncCommand
    public class ListNotesCommand : AsyncCommand
    {
        private readonly List<string> _notes;

        public ListNotesCommand(List<string> notes)
        {
            _notes = notes;
        }

        // Changed to ExecuteAsync, returns Task<int>
        public override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (_notes.Count == 0)
            {
                Console.WriteLine("No notes available.");
            }
            else
            {
                for (int i = 0; i < _notes.Count; i++)
                {
                    Console.WriteLine($"{i}: {_notes[i]}");
                }
            }
            return Task.FromResult(0); 
        }
    }
}