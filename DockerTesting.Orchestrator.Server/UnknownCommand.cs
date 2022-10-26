using System.Threading.Tasks;
using NamedPipesRpc.Core;
using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Server
{
    public class UnknownCommand : IServerCommand
    {
        private readonly string _name;

        public UnknownCommand(string name)
        {
            _name = name;
        }

        public Task ExecuteAsync(JObject request)
        {
            throw new UnknownCommandException(_name);
        }
    }
}