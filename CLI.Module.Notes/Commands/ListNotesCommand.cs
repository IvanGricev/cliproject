using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Threading;

namespace CLI.Module.Notes.Commands
{
    public class ListNotesCommand : Command
    {
        private readonly List<string> _notes;

        public ListNotesCommand(List<string> notes)
        {
            _notes = notes;
        }

        public override int Execute(CommandContext context, CancellationToken cancellationToken)
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
            return 0;
        }
    }
}