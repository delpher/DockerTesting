using Newtonsoft.Json.Linq;

namespace DockerTesting.Orchestrator.Api
{
    public static class AttachClientCommand
    {
        public const string Name = "ATTACH_CLIENT";

        public static object CreateRequest(int clientProcessId)
        {
            return new
            {
                command = Name,
                clientProcessId
            };
        }

        public static int ParseRequest(JObject request)
        {
            return int.Parse(request.SelectToken("clientProcessId").ToString());
        }
    }
}