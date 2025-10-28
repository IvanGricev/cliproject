namespace CLI.Core;

public interface ICommandModule
{
    String Name { get; }
    
    String Description { get; }

    void ShowHelp();

    // The method called for every command the user types inside your module
    void ProcessCommand(String input);
}
