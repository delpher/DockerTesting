using System.Threading.Tasks;
using DockerTesting.Orchestrator.Server.Processes;

namespace DockerTesting.Orchestrator.Server.Ports
{
    public class PortResource : IResource
    {
        private readonly PortPool _portPool;

        public int PortNumber { get; }

        public PortResource(PortPool portPool, int portNumber)
        {
            _portPool = portPool;
            PortNumber = portNumber;
        }

        public Task FreeAsync()
        {
            _portPool.Free(PortNumber);
            return Task.FromResult(true);
        }

        public override string ToString()
        {
            return "PORT " + PortNumber;
        }
    }
}