using CLI.Core; 
using CLI.Module.Notes.Commands;
using Spectre.Console.Cli;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CLI.Module.Notes
{
    public class NotesModule : ICommandModule
    {
        public string Name => "Notes";
        public string Description => "A module to manage personal notes.";

        private List<string> _notes;
        private readonly ICommandApp _app;
        
        private readonly JsonDataService _dataService;
        private const string _notesFileName = "notes.json";

        public NotesModule()
        {
            _dataService = new JsonDataService();

            _notes = _dataService.LoadData<List<string>>(_notesFileName) ?? new List<string>();

            var services = new ServiceCollection();
            services.AddSingleton(_notes);
            
            var registrar = new SpectreTypeRegistrar(services);

            _app = new CommandApp(registrar);

            _app.Configure(config =>
            {
                config.AddCommand<AddNoteCommand>("add")
                    .WithDescription("Adds a new note.");
                config.AddCommand<ListNotesCommand>("list")
                    .WithDescription("Lists all notes.");
                config.AddCommand<DeleteNoteCommand>("delete")
                    .WithDescription("Deletes a note by index.");
                config.AddCommand<EditNoteCommand>("edit")
                    .WithDescription("Edits a note by index.");

                config.SetApplicationName(""); 
                config.ValidateExamples();
            });
        }

        public void ShowHelp()
        {
            _app.Run(new[] { "--help" });
        }

        public void ProcessCommand(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            try
            {
                int result = _app.Run(args);

                if (result == 0)
                {
                    string command = args[0].ToLower();
                    if (command == "add" || command == "delete" || command == "edit")
                    {
                        _dataService.SaveData(_notesFileName, _notes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}

