using System.Diagnostics;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public class SystemProcesses
    {
        public virtual IProcess GetProcessById(int processId)
        {
            return new ProcessWrapper(Process.GetProcessById(processId));
        }

        private class ProcessWrapper : IProcess
        {
            private readonly Process _process;

            public ProcessWrapper(Process process)
            {
                _process = process;
            }

            public void WaitForExit()
            {
                _process.WaitForExit();
            }
        }
    }
}