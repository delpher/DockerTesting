using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client.Rpc;
using NamedPipesRpc.Core;

namespace DockerTesting.Orchestrator.Client.Commands
{
    internal class AttachClientCommand : RpcCommand
    {
        public AttachClientCommand(OrchestratorPipeFactory pipeFactory) : base(pipeFactory)
        {
        }

        public async Task ExecuteAsync(int processId)
        {
            using (var client = await CreateClientAsync())
                await client.SendMessageAsync(Api.AttachClientCommand.CreateRequest(processId));
        }
    }
}