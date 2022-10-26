using System.Diagnostics;
using System.IO;

namespace DockerTesting.Orchestrator.Server.Logging
{
    public static class Logger
    {
        public static void Log(string message, params object[] arguments)
        {
            File.AppendAllText("D:\\DockerTesting.Orchestrator.log",
                "[" + Process.GetCurrentProcess().Id + "] " + string.Format(message, arguments) + "\r\n");
        }
    }
}