using System.Diagnostics;

namespace DockerTesting.Orchestrator.Server.Utils
{
    public static class OrchestratorServerHost
    {
        private const string ServerHostProcessName = "DockerTesting.Orchestrator.Server.Host";
        private static readonly string ServerHostExeName;

        static OrchestratorServerHost()
        {
            ServerHostExeName = Path.Combine("DockerTesting.Orchestrator", "DockerTesting.Orchestrator.Server.Host.exe");
        }

        public static bool IsRunning()
        {
            return Process.GetProcessesByName(ServerHostProcessName).Any();
        }

        public static void Start(OrchestratorStartSettings startSettings)
        {
            var arguments = new List<string> { startSettings.ClientProcessId.ToString() };
            if (startSettings.ContinuousRun)
                arguments.Add("/C");
            if (startSettings.EnableLogging)
                arguments.Add("/L");

            var serverHost = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = ServerHostExeName,
                    UseShellExecute = false,
                    Arguments = string.Join(" ", arguments)
                }
            };

            serverHost.Start();
        }

        public static int CountRunning()
        {
            return Process
                .GetProcessesByName(ServerHostProcessName)
                .Count(p => p.Id != Process.GetCurrentProcess().Id);
        }
    }
}