using System.Linq;

namespace DockerTesting.Orchestrator.TestClient
{
    internal class ConsoleCommand
    {
        private readonly string[] _args;
        public static ConsoleCommand Empty => new ConsoleCommand(string.Empty, new string[0]);
        public string Name { get; }

        public static ConsoleCommand Parse(string userEntry)
        {
            var sp = userEntry.Split(' ');
            return new ConsoleCommand(sp[0], sp.Skip(1).ToArray());
        }

        private ConsoleCommand(string name, string[] args)
        {
            Name = name;
            _args = args;
        }

        public string StringArg(int argIndex)
        {
            return _args[argIndex];
        }

        public int IntArg(int argIndex)
        {
            return int.Parse(_args[argIndex]);
        }
    }
}