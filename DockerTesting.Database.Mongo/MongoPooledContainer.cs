using DockerTesting.Containers;
using DockerTesting.Containers.Pool;

namespace DockerTesting.Database.Mongo
{
    // ReSharper disable once UnusedType.Global
    public class MongoPooledContainer: PooledContainer
    {
        public MongoPooledContainer(string imageName, int? port) : base(imageName, port ?? 27017)
        {
        }

        protected override bool ContainerReadyOutputReceived(ContainerLogEventArgs args)
        {
            return args.Value.Contains("Waiting for connections");
        }
    }
}