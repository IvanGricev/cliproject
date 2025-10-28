using CLI.Core;
using System.Collections.Generic;

namespace CLI.Module.Notes
{
    public class NotesModule : CLI.Core.ICommandModule
    {
        public String Name => "Notes";
        public String Description => "A module to manage personal notes.";

        public List<String> _notes = new List<String>();

        public void ShowHelp()
        {
            Console.WriteLine("Notes Module Commands:");
            Console.WriteLine(" add <note>                  - Add a new note");
            Console.WriteLine(" list                        - List all notes");
            Console.WriteLine(" delete <index>              - Delete a note by its index");
            Console.WriteLine(" edit <noteId> <new content> - Edit a note by its index");
            Console.WriteLine(" module_exit                 - Exit the Notes module");
            Console.WriteLine(" exit                        - Exit the CLI application");
        }

        public void ProcessCommand(String input)
        {
            var parts = input.Split(' ', 2);
            var command = parts[0].ToLower();

            switch (command)
            {
                case "add":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Please provide a note to add.");
                    }
                    else
                    {
                        _notes.Add(parts[1]);
                        Console.WriteLine("Note added.");
                    }
                    break;

                case "list":
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
                    break;

                case "delete":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int deleteIndex) || deleteIndex < 0 || deleteIndex >= _notes.Count)
                    {
                        Console.WriteLine("Please provide a valid note index to delete.");
                    }
                    else
                    {
                        _notes.RemoveAt(deleteIndex);
                        Console.WriteLine("Note deleted.");
                    }
                    break;

                case "edit":
                    var editParts = parts.Length > 1 ? parts[1].Split(' ', 2) : new string[0];
                    if (editParts.Length < 2 || !int.TryParse(editParts[0], out int editIndex) || editIndex < 0 || editIndex >= _notes.Count)
                    {
                        Console.WriteLine("Please provide a valid note index and new content to edit.");
                    }
                    else
                    {
                        _notes[editIndex] = editParts[1];
                        Console.WriteLine("Note edited.");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
                    break;
            }
        }
    }

}

