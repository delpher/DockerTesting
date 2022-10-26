using System;
using System.Threading.Tasks;

namespace DockerTesting.Containers
{
    public interface IDockerContainer
    {
        Task StartAsync();
        Task DestroyAsync();
        string Id { get; }
        IContainerLog Output { get; }
    }
}