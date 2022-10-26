using DockerTesting.Containers;
using DockerTesting.Containers.Pool;

namespace DockerTesting.Database.Sql
{
    // ReSharper disable once UnusedType.Global
    public class SqlPooledDockerContainer : PooledContainer
    {
        public SqlPooledDockerContainer(string imageName, int? port) : base(imageName, port ?? 1433)
        {
        }

        protected override bool ContainerReadyOutputReceived(ContainerLogEventArgs args)
        {
            return args.Value.Contains("SQL Server is now ready for client connections.");
        }
    }
}