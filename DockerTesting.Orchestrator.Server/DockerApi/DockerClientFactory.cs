using System;
using Docker.DotNet;

namespace DockerTesting.Orchestrator.Server.DockerApi
{
    public class DockerClientFactory
    {
        public static DockerClient CreateDockerClient()
        {
            return new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }
    }
}