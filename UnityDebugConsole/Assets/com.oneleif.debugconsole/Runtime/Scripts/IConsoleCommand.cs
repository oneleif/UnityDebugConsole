namespace Oneleif.debugconsole
{
    public interface IConsoleCommand
    {
        string Command { get; }
        string Description { get; }
        string Help { get; }

        bool Process(string[] args);
    }
}