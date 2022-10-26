using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client.Rpc;
using NamedPipesRpc.Core;

namespace DockerTesting.Orchestrator.Client.Commands
{
    internal class WatchContainerCommand : RpcCommand
    {
        public WatchContainerCommand(OrchestratorPipeFactory pipeFactory) : base(pipeFactory)
        {
        }

        public async Task ExecuteAsync(int processId, string containerId, int portNumber)
        {
            using (var client = await CreateClientAsync())
                await client.SendMessageAsync(Api.WatchContainerCommand.CreateRequest(processId, containerId, portNumber));
        }
    }
}