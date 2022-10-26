using System.Collections.Generic;
using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public class ProcessRegistry : IProcessRegistry
    {
        private readonly Dictionary<int, List<IResource>> _resources;

        public ProcessRegistry()
        {
            _resources = new Dictionary<int, List<IResource>>();
        }

        public void AddResource(int processId, IResource resource)
        {
            _resources[processId].Add(resource);
        }

        public async Task RemoveAsync(int processId)
        {
            foreach (var resource in _resources[processId])
                await resource.FreeAsync();
            _resources.Remove(processId);
        }

        public bool Add(int processId)
        {
            if (_resources.ContainsKey(processId))
                return false;
            _resources.Add(processId, new List<IResource>());
            return true;
        }

        public bool Any()
        {
            return _resources.Count > 0;
        }
    }
}