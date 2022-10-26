using System.IO.Pipes;
using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Client.Rpc
{
    internal abstract class RpcCommand
    {
        private readonly OrchestratorPipeFactory _pipeFactory;

        protected RpcCommand(OrchestratorPipeFactory pipeFactory)
        {
            _pipeFactory = pipeFactory;
        }

        protected async Task<PipeStream> CreateClientAsync()
        {
            var client = _pipeFactory.CreatePipe();
            await client.ConnectAsync(10000);
            return client;
        }
    }

}