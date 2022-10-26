using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerTesting.Containers
{
    public class DockerContainerFactory
    {
        private const string DockerEngineUri = "npipe://./pipe/docker_engine";
        private readonly string _image;
        private readonly DockerClient _client;

        public DockerContainerFactory(string image)
        {
            _image = image;
            _client = new DockerClientConfiguration(new Uri(DockerEngineUri)).CreateClient();
        }

        public async Task<IDockerContainer> CreateContainerAsync(int hostPort, int containerPort)
        {
            var container = await CreateStoppedContainerAsync(hostPort, containerPort);
            await container.StartAsync();
            return container;
        }

        public Task<IDockerContainer> CreateStoppedContainerAsync(int hostPort, int containerPort)
        {
            return CreateStoppedContainerAsync(new[] { hostPort }, new[] { containerPort });
        }

        public async Task<IDockerContainer> CreateStoppedContainerAsync(IEnumerable<int> hostPort, IEnumerable<int> containerPort)
        {
            await EnsureImageExistsAsync();

            var createContainerParams = new CreateContainerParameters
            {
                Image = _image,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true,
                    PortBindings = containerPort.Select((port, index) => new KeyValuePair<string, IList<PortBinding>>(
                        $"{port}/tcp",
                        new List<PortBinding> {new PortBinding {HostPort = $"{hostPort.ElementAt(index)}"}}
                    )).ToDictionary(binding => binding.Key, binding => binding.Value)
                }
            };

            var response = await _client.Containers.CreateContainerAsync(createContainerParams);
            return new DockerContainer(_client, response.ID);
        }

        private async Task EnsureImageExistsAsync()
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters { All = true });
            if (!images.Any(i => i.RepoTags.Contains(_image)))
                throw new Exception($"Docker image '{_image}' not found. Build one before running tests.");
        }
    }
}