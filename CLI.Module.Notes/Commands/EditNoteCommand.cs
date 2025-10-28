using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

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

    public class EditNoteCommand : Command<EditNoteSettings>
    {
        private readonly List<string> _notes;

        public EditNoteCommand(List<string> notes)
        {
            _notes = notes;
        }

        public override int Execute(CommandContext context, EditNoteSettings settings, CancellationToken cancellationToken)
        {
            if (settings.Index < 0 || settings.Index >= _notes.Count)
            {
                Console.WriteLine("Please provide a valid note index.");
                return -1; // -1 means failure
            }
            if (settings.NewContent == null || settings.NewContent.Length == 0)
            {
                Console.WriteLine("Please provide new content for the note.");
                return -1; // -1 means failure
            }

            string fullContent = string.Join(" ", settings.NewContent);
            _notes[settings.Index] = fullContent;
            Console.WriteLine("Note edited.");
            return 0;
        }
    }
}