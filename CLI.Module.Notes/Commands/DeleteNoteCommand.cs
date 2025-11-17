using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CLI.Module.Notes.Commands
{
    public class DeleteNoteSettings : CommandSettings
    {
        [CommandArgument(0, "<INDEX>")]
        [Description("The 0-based index of the note to delete.")]
        public int Index { get; set; }
    }

    public class DeleteNoteCommand : AsyncCommand<DeleteNoteSettings>
    {
        private readonly List<string> _notes;

        public DeleteNoteCommand(List<string> notes)
        {
            _notes = notes;
        }

        public override Task<int> ExecuteAsync(CommandContext context, DeleteNoteSettings settings, CancellationToken cancellationToken)
        {
            if (settings.Index < 0 || settings.Index >= _notes.Count)
            {
                Console.WriteLine("Please provide a valid note index.");
                return Task.FromResult(-1); 
            }

            _notes.RemoveAt(settings.Index);
            Console.WriteLine("Note deleted.");
            return Task.FromResult(0); 
        }
    }
}