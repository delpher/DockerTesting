using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using DockerTesting.Orchestrator.Server.Logging;
using DockerTesting.Orchestrator.Server.Utils;

namespace DockerTesting.Orchestrator.Server.Host
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void Main(string[] args)
        {
            HideMainWindow();

            if (OrchestratorServerHost.CountRunning() > 1)
                return;

            var startSettings = CreateStartSettings(args);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                if (startSettings.EnableLogging)
                    Logger.Log("FAILURE: {0}", eventArgs.ExceptionObject.ToString());
            };

            OrchestratorServer.Run(startSettings).Wait();
        }

        private static void HideMainWindow()
        {
#if !DEBUG
            var windowHandle = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(windowHandle, 0);
#endif
        }

        private static OrchestratorStartSettings CreateStartSettings(IReadOnlyList<string> arguments)
        {
            return new OrchestratorStartSettings
            {
                ContinuousRun = arguments.Contains("/C"),
                EnableLogging = arguments.Contains("/L"),
                ClientProcessId = int.Parse(arguments[0])
            };
        }
    }
}