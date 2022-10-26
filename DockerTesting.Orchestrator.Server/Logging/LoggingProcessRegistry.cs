using System;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Server.Processes;

namespace DockerTesting.Orchestrator.Server.Logging
{
    public class LoggingProcessRegistry : IProcessRegistry
    {
        private readonly ProcessRegistry _processRegistry;

        public LoggingProcessRegistry(ProcessRegistry processRegistry)
        {
            _processRegistry = processRegistry;
        }

        public void AddResource(int processId, IResource resource)
        {
            Log("Process {0} Adding resource {1}", processId, resource.ToString());
            _processRegistry.AddResource(processId, resource);
        }

        public async Task RemoveAsync(int processId)
        {
            Log("Unregistering process: {0}", processId);

            try
            {
                await _processRegistry
                    .RemoveAsync(processId)
                    .ContinueWith(task => Log("Complete: {0} {1}", task.Status, task.Exception?.ToString()));
                Log("Done.");
            }
            catch (Exception ex)
            {
                Log("Failed. {0}", ex.ToString());
            }
        }

        public bool Add(int processId)
        {
            Log("Registering process {0}", processId);

            var result = _processRegistry.Add(processId);

            Log(result ? "Done" : "Already registered");
            return result;
        }

        public bool Any()
        {
            return _processRegistry.Any();
        }

        private void Log(string message, params object[] arguments)
        {
            Logger.Log("PROCESS REGISTRY: " + message, arguments);
        }
    }
}