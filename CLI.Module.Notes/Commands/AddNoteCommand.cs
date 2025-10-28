using Spectre.Console.Cli;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace CLI.Module.Notes.Commands
{
    public class AddNoteSettings : CommandSettings
    {
        [CommandArgument(0, "<NOTE_TEXT...>")]
        [Description("The text of the note to add. No quotes needed.")]
        public string[] NoteText { get; set; }
    }

    public class AddNoteCommand : Command<AddNoteSettings>
    {
        private readonly List<string> _notes;

        public AddNoteCommand(List<string> notes)
        {
            _notes = notes;
        }

        public override int Execute(CommandContext context, AddNoteSettings settings, CancellationToken cancellationToken)
        {
            if (settings.NoteText == null || settings.NoteText.Length == 0)
            {
                Console.WriteLine("Please provide a note to add.");
                return -1; // -1 means failure
            }

            string fullNote = string.Join(" ", settings.NoteText);
            _notes.Add(fullNote);
            Console.WriteLine("Note added.");
            return 0; // 0 means success
        }
    }
}