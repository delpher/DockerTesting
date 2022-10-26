using System;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Client;

namespace DockerTesting.Containers.Pool
{
    public abstract class PooledContainer
    {
        private readonly int _port;
        private readonly DockerContainerFactory _dockerContainerFactory;
        private IDockerContainer _container;

        private int? _hostPortNumber;

        protected PooledContainer(string imageName, int port)
        {
            _port = port;
            _dockerContainerFactory = new DockerContainerFactory(imageName);
        }

        public async Task Start()
        {
            var orchestrator = await OrchestratorClient.StartAsync();
            _hostPortNumber = await orchestrator.SpawnPortAsync();
            _container = await _dockerContainerFactory.CreateStoppedContainerAsync(_hostPortNumber.Value, _port);
            await orchestrator.WatchContainerAsync(_container.Id, _hostPortNumber.Value);
            await StartContainerAndWaitReady();
        }

        private async Task StartContainerAndWaitReady()
        {
            var tcs = new TaskCompletionSource<bool>();

            void Handler(object _, ContainerLogEventArgs args)
            {
                if (ContainerReadyOutputReceived(args)) tcs.TrySetResult(true);
            }

            _container.Output.OutputReceived += Handler;

            await _container.StartAsync();

            await tcs.Task.ContinueWith(_ => _container.Output.OutputReceived -= Handler);
        }

        protected abstract bool ContainerReadyOutputReceived(ContainerLogEventArgs args);

        public async Task Destroy()
        {
            await _container.DestroyAsync();
        }

        public int GetHostPort()
        {
            if (_hostPortNumber == null) throw ContainerNotInitialized();

            return _hostPortNumber.Value;
        }

        private static InvalidOperationException ContainerNotInitialized()
        {
            return new InvalidOperationException(
                $"Container was not initialized. You should call {nameof(Start)} first.");
        }
    }
}