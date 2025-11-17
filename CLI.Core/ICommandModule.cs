using System.Threading.Tasks;

namespace CLI.Core
{
    public interface ICommandModule
    {
        string Name { get; }
        string Description { get; }

        Task ShowHelpAsync();

        Task ProcessCommandAsync(string input);
    }
}