using System.IO.Pipes;
using DockerTesting.Orchestrator.Api;

namespace DockerTesting.Orchestrator.Client
{
    internal class OrchestratorPipeFactory
    {
        public NamedPipeClientStream CreatePipe()
        {
            return new NamedPipeClientStream(".", OrchestratorPipe.Name, PipeDirection.InOut);
        }
    }
}