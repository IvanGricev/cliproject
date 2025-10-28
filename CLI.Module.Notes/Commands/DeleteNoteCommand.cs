using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace CLI.Module.Notes.Commands
{
    public class DeleteNoteSettings : CommandSettings
    {
        [CommandArgument(0, "<INDEX>")]
        [Description("The 0-based index of the note to delete.")]
        public int Index { get; set; }
    }

    public class DeleteNoteCommand : Command<DeleteNoteSettings>
    {
        private readonly List<string> _notes;

        public DeleteNoteCommand(List<string> notes)
        {
            _notes = notes;
        }

        public override int Execute(CommandContext context, DeleteNoteSettings settings, CancellationToken cancellationToken)
        {
            if (settings.Index < 0 || settings.Index >= _notes.Count)
            {
                Console.WriteLine("Please provide a valid note index.");
                return -1; // -1 means failure
            }

            _notes.RemoveAt(settings.Index);
            Console.WriteLine("Note deleted.");
            return 0;
        }
    }
}