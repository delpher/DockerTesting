using System;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client;

namespace DockerTesting.Orchestrator.TestClient
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(RunClient).Wait();
        }

        private static async Task RunClient()
        {
            var client = await OrchestratorClient.StartAsync();
            var command = ConsoleCommand.Empty;

            while (command.Name != "exit")
            {
                Console.Write("> ");
                command = ConsoleCommand.Parse(Console.ReadLine());

                switch (command.Name)
                {
                    case "sp":
                    {
                        var port = await client.SpawnPortAsync();
                        Console.WriteLine("SPAWNED PORT: {0}", port);
                        break;
                    }
                    case "wc":
                    {
                        var containerId = command.StringArg(0);
                        var portNumber = command.IntArg(1);
                        await client.WatchContainerAsync(containerId, portNumber);
                        Console.WriteLine("STARTED WATCHING CONTAINER: {0}", containerId);
                        break;
                    }
                }
            }
        }
    }
}