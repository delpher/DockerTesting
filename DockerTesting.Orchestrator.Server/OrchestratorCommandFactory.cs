using System.IO.Pipes;
using DockerTesting.Orchestrator.Api;
using DockerTesting.Orchestrator.Server.Ports;
using DockerTesting.Orchestrator.Server.Processes;
using Newtonsoft.Json.Linq;
using AttachClientCommand = DockerTesting.Orchestrator.Api.AttachClientCommand;
using SpawnPortCommand = DockerTesting.Orchestrator.Api.SpawnPortCommand;

namespace DockerTesting.Orchestrator.Server
{
    public class OrchestratorCommandFactory
    {
        private readonly ProcessWatchdog _processWatchdog;
        private readonly PortPool _portPool;

        public OrchestratorCommandFactory(ProcessWatchdog processWatchdog, PortPool portPool)
        {
            _processWatchdog = processWatchdog;
            _portPool = portPool;
        }

        public IServerCommand CreateCommand(JObject message, PipeStream pipe)
        {
            var command = message.SelectToken("command")?.ToString();
            switch (command)
            {
                case AttachClientCommand.Name:
                    return new Processes.AttachClientCommand(_processWatchdog);
                case SpawnPortCommand.Name:
                    return new Ports.SpawnPortCommand(_processWatchdog, _portPool, pipe);
                case WatchContainerCommand.Name:
                    return new Containers.WatchContainerCommand(_processWatchdog, _portPool);
                default:
                    return new UnknownCommand(command);
            }
        }
    }
}