using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DockerTesting.Orchestrator.Server.DockerApi;
using DockerTesting.Orchestrator.Server.Processes;
using NamedPipesRpc.Core;
using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Server.Ports
{
    public class SpawnPortCommand : IServerCommand
    {
        private readonly ProcessWatchdog _processWatchdog;
        private readonly PortPool _portPool;
        private readonly PipeStream _pipe;

        public SpawnPortCommand(ProcessWatchdog processWatchdog, PortPool portPool, PipeStream pipe)
        {
            _processWatchdog = processWatchdog;
            _portPool = portPool;
            _pipe = pipe;
        }

        public async Task ExecuteAsync(JObject request)
        {
            var processId = Api.SpawnPortCommand.ParseRequest(request);
            PortResource portResource;

            do
            {
                portResource = _portPool.GetOne();
                _processWatchdog.WatchResource(processId, portResource);
            } while (await CheckPortIsInUseAsync(portResource.PortNumber));

            await _pipe.SendMessageAsync(Api.SpawnPortCommand.CreateResponse(portResource.PortNumber));
        }

        private async Task<bool> CheckPortIsInUseAsync(int portNumber)
        {
            var dockerClient = DockerClientFactory.CreateDockerClient();
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            return containers.Any(c => c.Ports.Any(port => port.PublicPort == portNumber));
        }
    }
}