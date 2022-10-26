using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client.Rpc;
using NamedPipesRpc.Core;

namespace DockerTesting.Orchestrator.Client.Commands
{
    internal class SpawnPortCommand : RpcCommand
    {
        public SpawnPortCommand(OrchestratorPipeFactory pipeFactory) : base(pipeFactory)
        {
        }

        public async Task<int> ExecuteAsync(int processId)
        {
            using (var client = await CreateClientAsync())
            {
                await client.SendMessageAsync(Api.SpawnPortCommand.CreateRequest(processId));
                return Api.SpawnPortCommand.ParseResponse(await client.ReadMessageAsync());
            }
        }
    }
}