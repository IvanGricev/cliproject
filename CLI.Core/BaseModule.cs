using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
using System.Linq;

namespace CLI.Core
{
    /// <summary>
    /// An abstract base class for all modules.
    /// Handles all boilerplate setup for DI, Spectre, and JsonDataService.
    /// </summary>
    public abstract class BaseModule : ICommandModule
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        protected readonly ICommandApp App;
        protected readonly JsonDataService DataService;
        protected readonly IConfigurationService ConfigService;

        protected BaseModule()
        {
            // 1. Initialize all common services
            DataService = new JsonDataService();
            ConfigService = new ConfigurationService(DataService);

            // 2. Set up Dependency Injection
            var services = new ServiceCollection();
            services.AddSingleton(DataService);
            services.AddSingleton(ConfigService);
            
            // Allow child modules to add their own services
            ConfigureServices(services);

            // 3. Set up Spectre.Console.Cli
            var registrar = new SpectreTypeRegistrar(services);
            App = new CommandApp(registrar);
            App.Configure(config =>
            {
                // Allow child modules to configure their commands
                ConfigureCommands(config);

                config.SetApplicationName(""); // Hides the app name from help
                config.ValidateExamples();
            });
        }

        /// <summary>
        /// Main entry point from the shell. Forwards input to Spectre.
        /// </summary>
        public virtual void ProcessCommand(string input)
        {
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return;

            try
            {
                int result = App.Run(args);

                // If a command (not help) succeeded, call the OnCommandExecuted hook
                if (result == 0)
                {
                    var command = args.First().ToLower();
                    if (command != "--help" && command != "-h")
                    {
                        OnCommandExecuted(command, args);
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

        /// <summary>
        /// Forwards "help" to Spectre.
        /// </summary>
        public virtual void ShowHelp()
        {
            App.Run(new[] { "--help" });
        }

        // --- Abstract and Virtual methods for child classes ---

        /// <summary>
        /// Child modules MUST implement this to add their commands.
        /// </summary>
        protected abstract void ConfigureCommands(IConfigurator config);

        /// <summary>
        /// Child modules CAN override this to add their own services to DI
        /// (e.g., `services.AddSingleton(myNotesList);`).
        /// </summary>
        protected virtual void ConfigureServices(IServiceCollection services) 
        { 
            // Does nothing by default
        }

        /// <summary>
        /// Child modules CAN override this to react to a successful command
        /// (e.g., to save data).
        /// </summary>
        protected virtual void OnCommandExecuted(string commandName, string[] args)
        {
            // Does nothing by default
        }
    }
}

