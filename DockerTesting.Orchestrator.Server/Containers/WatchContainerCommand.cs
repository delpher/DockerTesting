using System.Threading.Tasks;
using DockerTesting.Orchestrator.Server.Ports;
using DockerTesting.Orchestrator.Server.Processes;
using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Server.Containers
{
    public class WatchContainerCommand : IServerCommand
    {
        private readonly ProcessWatchdog _processWatchdog;
        private readonly PortPool _portPool;

        public WatchContainerCommand(ProcessWatchdog processWatchdog, PortPool portPool)
        {
            _processWatchdog = processWatchdog;
            _portPool = portPool;
        }
        public Task ExecuteAsync(JObject request)
        {
            var parameters = Api.WatchContainerCommand.ParseRequest(request);
            var containerResource = CreateContainerResource(parameters.ContainerId, _portPool.GetOne(parameters.PortNumber));
            _processWatchdog.WatchResource(parameters.ProcessId, containerResource);

            return Task.FromResult(true);
        }

        private ContainerResource CreateContainerResource(string containerId, PortResource port)
        {
            return new ContainerResource(containerId, port);
        }
    }
}