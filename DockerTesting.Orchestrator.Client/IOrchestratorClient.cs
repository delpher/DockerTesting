using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Client
{
    public interface IOrchestratorClient
    {
        Task<int> SpawnPortAsync();
        Task WatchContainerAsync(string containerId, int portNumber);
    }
}