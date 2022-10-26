using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DockerTesting.Containers;
using DockerTesting.Orchestrator.Client;
using DockerTesting.Orchestrator.Server.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DockerTesting.IntegrationTests
{
    public class DockerTestingIntegrationTestsCollection
    {
        private readonly ITestOutputHelper _output;
        private const string NotExistingImageName = "not-existing-image";
        private const string TestImageName = "docker/getting-started:latest";
        private const string TestUrl = "http://localhost:{0}/tutorial/";
        private const int TestContainerPort = 80;

        public DockerTestingIntegrationTestsCollection(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Should_Throw_When_Image_Not_Found()
        {
            var factory = new DockerContainerFactory(NotExistingImageName);
            Func<Task> createContainer = async () => await factory.CreateContainerAsync(0, 0);

            (await createContainer.Should().ThrowAsync<Exception>())
                .Which.Message.Should().Contain(NotExistingImageName);
        }

        [Fact]
        public async Task Should_Return_Console_Output()
        {
            var orchestratorClient =
                await OrchestratorClient.StartAsync(new OrchestratorStartSettings { EnableLogging = true });
            var hostPort = await orchestratorClient.SpawnPortAsync();

            var factory = new DockerContainerFactory(TestImageName);
            var container = await factory.CreateStoppedContainerAsync(hostPort, TestContainerPort);

            container.Output.OutputReceived += (sender, args) => _output.WriteLine("CONTAINER OUT: " + args.Value);

            using (var outputMonitor = container.Output.Monitor())
            {
                await container.StartAsync();

                await orchestratorClient.WatchContainerAsync(container.Id, hostPort);

                var httpClient = new HttpClient();

                HttpResponseMessage response;
                do
                {
                    await Task.Delay(1000);
                    response = await httpClient.GetAsync(string.Format(TestUrl, hostPort));
                } while (response == null);

                outputMonitor.Should().Raise(nameof(container.Output.OutputReceived))
                    .WithArgs<ContainerLogEventArgs>(args =>
                        args.Value.Contains("/docker-entrypoint.sh: Configuration complete; ready for start up"));
            }

            await container.DestroyAsync();
        }

        [Fact]
        public async Task Should_Allow_To_Call_Destroy_Multiple_Times()
        {
            var orchestratorClient =
                await OrchestratorClient.StartAsync(new OrchestratorStartSettings { EnableLogging = true });
            var hostPort = await orchestratorClient.SpawnPortAsync();

            var factory = new DockerContainerFactory(TestImageName);
            var container = await factory.CreateStoppedContainerAsync(hostPort, TestContainerPort);

            await container.StartAsync();
            await container.DestroyAsync();

            Func<Task> destroy = () => container.DestroyAsync();

            await destroy.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task Should_Run_Docker_Image_In_Container()
        {
            var orchestratorClient =
                await OrchestratorClient.StartAsync(new OrchestratorStartSettings { EnableLogging = true });
            var hostPort = await orchestratorClient.SpawnPortAsync();

            var factory = new DockerContainerFactory(TestImageName);
            var container = await factory.CreateContainerAsync(hostPort, TestContainerPort);

            await orchestratorClient.WatchContainerAsync(container.Id, hostPort);

            await Task.Delay(1000);

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(string.Format(TestUrl, hostPort));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await container.DestroyAsync();
        }
    }
}