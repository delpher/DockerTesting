using System;
using System.Threading.Tasks;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public class ProcessWatchdog
    {
        private readonly SystemProcesses _processes;
        private readonly IProcessRegistry _clientProcesses;

        public event EventHandler LastClientExited;

        public ProcessWatchdog(IProcessRegistry clientProcesses, SystemProcesses processes = null)
        {
            _clientProcesses = clientProcesses;
            _processes = processes ?? new SystemProcesses();
        }

        public void WatchResource(int processId, IResource resource)
        {
            if (_clientProcesses.Add(processId))
                WatchClientProcess(processId);
            _clientProcesses.AddResource(processId, resource);
        }

        public virtual void WatchClientProcess(int processId)
        {
            var clientProcess = _processes.GetProcessById(processId);
            _clientProcesses.Add(processId);
            Task.Factory.StartNew(() => clientProcess.WaitForExit(), TaskCreationOptions.LongRunning)
                .ContinueWith(async t => await ProcessExitedAsync(processId));
        }

        protected virtual async Task ProcessExitedAsync(int clientProcessId)
        {
            await _clientProcesses.RemoveAsync(clientProcessId);
            if (!HasProcessesToWatch())
                OnLastProcessExited();
        }

        protected virtual void OnLastProcessExited()
        {
            LastClientExited?.Invoke(this, EventArgs.Empty);
        }

        public bool HasProcessesToWatch()
        {
            return _clientProcesses.Any();
        }
    }
}