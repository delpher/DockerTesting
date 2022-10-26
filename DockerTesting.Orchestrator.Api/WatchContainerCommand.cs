using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Api
{
    public static class WatchContainerCommand
    {
        public const string Name = "WATCH_CONTAINER";

        public static object CreateRequest(int processId, string containerId, int portNumber)
        {
            return new
            {
                command = Name,
                processId,
                containerId,
                portNumber
            };
        }

        public static Request ParseRequest(JObject request)
        {
            return request.ToObject<Request>();
        }

        public class Request
        {
            public int ProcessId { get; set; }
            public string ContainerId { get; set; }

            public int PortNumber { get; set; }
        }
    }
}