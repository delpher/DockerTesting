using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public interface IProcessRegistry
    {
        void AddResource(int processId, IResource resource);
        Task RemoveAsync(int processId);
        bool Add(int processId);
        bool Any();
    }
}