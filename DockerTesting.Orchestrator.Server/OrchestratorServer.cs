using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Api;
using DockerTesting.Orchestrator.Server.Logging;
using DockerTesting.Orchestrator.Server.Ports;
using DockerTesting.Orchestrator.Server.Processes;
using DockerTesting.Orchestrator.Server.Utils;
using NamedPipesRpc.Core;

namespace DockerTesting.Orchestrator.Server
{
    public static class OrchestratorServer
    {
        public static async Task Run(OrchestratorStartSettings startSettings)
        {
            var cts = new CancellationTokenSource();
            var watcher = StartProcessWatchdog(startSettings, () =>
            {
                if (startSettings.ContinuousRun)
                    return;
                cts.Cancel();
                Environment.Exit(0);
            });

            var commandFactory = new OrchestratorCommandFactory(watcher, new PortPool());

            while (!cts.IsCancellationRequested || startSettings.ContinuousRun)
                using (var pipeServer = CreateServerNamedPipe())
                {
                    await pipeServer.WaitForConnectionAsync(cts.Token);
                    var message = await pipeServer.ReadMessageAsync();
                    var command = commandFactory.CreateCommand(message, pipeServer);
                    await command.ExecuteAsync(message);
                }
        }

        private static NamedPipeServerStream CreateServerNamedPipe()
        {
            return new NamedPipeServerStream(OrchestratorPipe.Name, PipeDirection.InOut);
        }

        private static ProcessWatchdog StartProcessWatchdog(OrchestratorStartSettings startSettings, Action onLastClientExited)
        {
            ProcessWatchdog watcher = CreateProcessWatchdog(startSettings);
            watcher.LastClientExited += (sender, args) => onLastClientExited();
            watcher.WatchClientProcess(startSettings.ClientProcessId);
            return watcher;
        }

        private static ProcessWatchdog CreateProcessWatchdog(OrchestratorStartSettings startSettings)
        {
            return startSettings.EnableLogging
                ? CreateProcessWatchdogWithLogging()
                : CreateProcessWatchdogNoLogging();
        }

        private static ProcessWatchdog CreateProcessWatchdogNoLogging()
        {
            return new ProcessWatchdog(new ProcessRegistry(), new SystemProcesses());
        }

        private static ProcessWatchdog CreateProcessWatchdogWithLogging()
        {
            return new LoggingProcessWatchdog(new LoggingProcessRegistry(new ProcessRegistry()), new SystemProcesses());
        }
    }
}