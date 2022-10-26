using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Server
{
    public interface IServerCommand
    {
        Task ExecuteAsync(JObject request);
    }
}