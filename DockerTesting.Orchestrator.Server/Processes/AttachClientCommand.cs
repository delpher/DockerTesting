using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Server.Processes
{
    public class AttachClientCommand : IServerCommand
    {
        private readonly ProcessWatchdog _processWatchdog;

        public AttachClientCommand(ProcessWatchdog processWatchdog)
        {
            _processWatchdog = processWatchdog;
        }

        public Task ExecuteAsync(JObject request)
        {
            var clientProcessId = Api.AttachClientCommand.ParseRequest(request);
            _processWatchdog.WatchClientProcess(clientProcessId);

            return Task.FromResult(true);
        }
    }
}