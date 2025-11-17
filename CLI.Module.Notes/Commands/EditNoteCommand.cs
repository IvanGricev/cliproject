using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks; 

namespace CLI.Module.Notes.Commands
{
    public class EditNoteSettings : CommandSettings
    {
        [CommandArgument(0, "<INDEX>")]
        [Description("The 0-based index of the note to edit.")]
        public int Index { get; set; }

        [CommandArgument(1, "<NEW_CONTENT...>")]
        [Description("The new text for the note.")]
        public string[] NewContent { get; set; }
    }

    // Changed to AsyncCommand
    public class EditNoteCommand : AsyncCommand<EditNoteSettings>
    {
        private readonly List<string> _notes;

        // This is the constructor, make sure it has parentheses ()
        public EditNoteCommand(List<string> notes)
        {
            _notes = notes;
        }

        // Changed to ExecuteAsync, returns Task<int>
        public override Task<int> ExecuteAsync(CommandContext context, EditNoteSettings settings, CancellationToken cancellationToken)
        {
            if (settings.Index < 0 || settings.Index >= _notes.Count)
            {
                Console.WriteLine("Please provide a valid note index.");
                return Task.FromResult(-1);
            }
            if (settings.NewContent == null || settings.NewContent.Length == 0)
            {
                Console.WriteLine("Please provide new content for the note.");
                return Task.FromResult(-1);
            }

            string fullContent = string.Join(" ", settings.NewContent);
            _notes[settings.Index] = fullContent;
            Console.WriteLine("Note edited.");
            return Task.FromResult(0);
        }
    }
}