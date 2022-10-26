using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Api
{
    public static class SpawnPortCommand
    {
        public const string Name = "ADD_PORT";

        public static object CreateRequest(int processId)
        {
            return new
            {
                command = Name,
                processId
            };
        }

        public static object CreateResponse(int port)
        {
            return new
            {
                port
            };
        }

        public static int ParseResponse(JObject response)
        {
            return int.Parse(response.SelectToken("port").ToString());
        }

        public static int ParseRequest(JObject request)
        {
            return int.Parse(request.SelectToken("processId").ToString());
        }
    }
}