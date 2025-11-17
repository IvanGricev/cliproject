using CLI.Core;
using CLI.Module.Notes.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLI.Module.Notes
{
    public class NotesModule : BaseModule
    {
        public override string Name => "Notes";
        public override string Description => "A module to manage personal notes.";

        private List<string> _notes;
        private const string _notesFileName = "notes.json";

        public NotesModule() : base() 
        {
        }

        protected override void ConfigureCommands(IConfigurator config)
        {
            config.AddCommand<AddNoteCommand>("add")
                  .WithDescription("Adds a new note.");
            config.AddCommand<ListNotesCommand>("list")
                  .WithDescription("Lists all notes.");
            config.AddCommand<DeleteNoteCommand>("delete")
                  .WithDescription("Deletes a note by index.");
            config.AddCommand<EditNoteCommand>("edit")
                  .WithDescription("Edits a note by index.");
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            _notes = DataService.LoadDataAsync<List<string>>(_notesFileName).GetAwaiter().GetResult() 
                     ?? new List<string>();

            services.AddSingleton(_notes);
        }

        protected override async Task OnCommandExecutedAsync(string commandName, string[] args)
        {
            if (commandName == "add" || commandName == "delete" || commandName == "edit")
            {
                await DataService.SaveDataAsync(_notesFileName, _notes);
            }
        }
    }
}