using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerTesting.Containers
{
    internal class DockerContainer : IDockerContainer
    {
        private readonly DockerClient _client;
        private readonly ContainerLogOutput _output;

        public string Id { get; }

        public IContainerLog Output => _output;

        public DockerContainer(DockerClient client, string containerId)
        {
            _client = client;
            Id = containerId;
            _output = new ContainerLogOutput();
        }

        public async Task StartAsync()
        {
            try
            {
                await _client.Containers.StartContainerAsync(Id, new ContainerStartParameters());
                _output.Attach(_client, Id);
            }
            catch
            {
                await DestroyAsync();
                throw;
            }
        }

        public async Task DestroyAsync()
        {
            _output.Release();
            try
            {
                await _client.Containers.StopContainerAsync(Id, new ContainerStopParameters());
                await _client.Containers.RemoveContainerAsync(Id, new ContainerRemoveParameters());
            }
            catch (DockerContainerNotFoundException)
            {
                // ignore
            }
        }

        private class ContainerLogOutput : IProgress<string>, IContainerLog
        {
            private CancellationTokenSource _cts;

            public void Report(string value)
            {
                OnOutputReceived(new ContainerLogEventArgs(value));
            }

            public event EventHandler<ContainerLogEventArgs> OutputReceived;

            protected virtual void OnOutputReceived(ContainerLogEventArgs e)
            {
                OutputReceived?.Invoke(this, e);
            }

            public void Attach(IDockerClient client, string containerId)
            {
                _cts = new CancellationTokenSource();
                var containerLogsParameters = new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = true
                };

                client.Containers.GetContainerLogsAsync(containerId, containerLogsParameters, _cts.Token, this);
            }

            public void Release()
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }
    }

    public interface IContainerLog
    {
        event EventHandler<ContainerLogEventArgs> OutputReceived;
    }

    public class ContainerLogEventArgs
    {
        public string Value { get; }

        public ContainerLogEventArgs(string value)
        {
            Value = value;
        }
    }
}