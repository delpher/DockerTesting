using System;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Server.Processes;

namespace DockerTesting.Orchestrator.Server.Logging
{
    public class LoggingProcessWatchdog : ProcessWatchdog
    {
        public LoggingProcessWatchdog(
            IProcessRegistry processRegistry,
            SystemProcesses processes = null)
            : base(processRegistry, processes)
        {
        }

        protected override Task ProcessExitedAsync(int clientProcessId)
        {
            try
            {
                Log("Client exited {0}", clientProcessId);
                return base.ProcessExitedAsync(clientProcessId);
            }
            catch (Exception ex)
            {
                Log("Failure.", ex.ToString());
                throw;
            }
        }

        public override void WatchClientProcess(int processId)
        {
            Log("Watching client {0}", processId);
            base.WatchClientProcess(processId);
        }

        protected override void OnLastProcessExited()
        {
            Log("Last client exited.");
            base.OnLastProcessExited();
        }

        private static void Log(string message, params object[] arguments)
        {
            Logger.Log("WATCHDOG: " + message, arguments);
        }
    }
}