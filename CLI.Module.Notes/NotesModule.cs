using CLI.Core;
using CLI.Module.Notes.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CLI.Module.Notes
{
    public class NotesModule : ICommandModule
    {
        public string Name => "Notes";
        public string Description => "A module to manage personal notes.";

        private readonly string _notesFilePath;
        private List<string> _notes; 

        private readonly CommandApp _app;

        public NotesModule()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "CLIShell");
            Directory.CreateDirectory(appFolder);
            _notesFilePath = Path.Combine(appFolder, "notes.json");
            
            LoadNotes();

            var services = new ServiceCollection();
            services.AddSingleton(_notes);
            var registrar = new DependencyInjectionRegistrar(services);

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
                    if (command != "list" && command != "--help" && command != "-h")
                    {
                        SaveNotes();
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
        
        private void LoadNotes()
        {
            try
            {
                if (File.Exists(_notesFilePath))
                {
                    string json = File.ReadAllText(_notesFilePath);
                    _notes = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
                else
                {
                    _notes = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notes: {ex.Message}");
                _notes = new List<string>();
            }
        }

        private void SaveNotes()
        {
            try
            {
                string json = JsonSerializer.Serialize(_notes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_notesFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving notes: {ex.Message}");
            }
        }
    }
}
