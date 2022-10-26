using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public interface IResource
    {
        Task FreeAsync();
    }
}