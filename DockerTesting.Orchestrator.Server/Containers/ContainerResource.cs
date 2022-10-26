using System.Net;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerTesting.Orchestrator.Server.DockerApi;
using DockerTesting.Orchestrator.Server.Ports;
using DockerTesting.Orchestrator.Server.Processes;

namespace DockerTesting.Orchestrator.Server.Containers
{
    public class ContainerResource : IResource
    {
        private readonly string _containerId;
        private readonly PortResource _portResource;

        public ContainerResource(string containerId, PortResource portResource)
        {
            _containerId = containerId;
            _portResource = portResource;
        }

        public async Task FreeAsync()
        {
            var client = DockerClientFactory.CreateDockerClient();

            try
            {
                await client.Containers.StopContainerAsync(_containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 3 });
                await client.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters { Force = true });
            }
            catch (DockerApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }
            finally
            {
                await _portResource.FreeAsync();
            }
        }

        public override string ToString()
        {
            return "CONTAINER " + _containerId;
        }
    }
}