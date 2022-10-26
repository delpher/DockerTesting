using System.Diagnostics;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client.Commands;
using DockerTesting.Orchestrator.Server.Utils;

namespace DockerTesting.Orchestrator.Client
{
    public class OrchestratorClient : IOrchestratorClient
    {
        private readonly OrchestratorPipeFactory _clientPipeFactory;

        public static async Task<IOrchestratorClient> StartAsync(OrchestratorStartSettings startSettings = null)
        {
            var client = new OrchestratorClient(new OrchestratorPipeFactory());
            startSettings = startSettings ?? new OrchestratorStartSettings();

            startSettings.ClientProcessId = GetCurrentProcessId();

            if (OrchestratorServerHost.IsRunning())
                await client.AttachProcessAsync(startSettings.ClientProcessId);
            else
                OrchestratorServerHost.Start(startSettings);

            return client;
        }

        private OrchestratorClient(OrchestratorPipeFactory clientPipeFactory)
        {
            _clientPipeFactory = clientPipeFactory;
        }

        public async Task<int> SpawnPortAsync()
        {
            var command = new SpawnPortCommand(_clientPipeFactory);
            return await command.ExecuteAsync(GetCurrentProcessId());
        }

        public async Task WatchContainerAsync(string containerId, int portNumber)
        {
            var command = new WatchContainerCommand(_clientPipeFactory);
            await command.ExecuteAsync(GetCurrentProcessId(), containerId, portNumber);
        }

        private async Task AttachProcessAsync(int processId)
        {
            var command = new AttachClientCommand(_clientPipeFactory);
            await command.ExecuteAsync(processId);
        }

        private static int GetCurrentProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }
    }
}