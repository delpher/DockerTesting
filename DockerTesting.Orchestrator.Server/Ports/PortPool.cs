using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerTesting.Orchestrator.Server.Ports
{
    public class PortPool
    {
        private readonly List<PortResource> _issuedPorts = new List<PortResource>();

        private const int StartPort = 1400;

        private const int EndPort = 1500;

        public PortResource GetOne(int portNumber)
        {
            return _issuedPorts.Single(resource => resource.PortNumber == portNumber);
        }

        public PortResource GetOne()
        {
            for (var port = StartPort; port < EndPort; port++)
            {
                if (HasPort(port))
                    continue;
                var newPortResource = new PortResource(this, port);
                _issuedPorts.Add(newPortResource);
                return newPortResource;
            }

            throw new PortPoolExhaustedException();
        }

        public void Free(int portNumber)
        {
            if (HasPort(portNumber))
                _issuedPorts.Remove(_issuedPorts.Single(resource => resource.PortNumber == portNumber));
        }

        private bool HasPort(int port)
        {
            return _issuedPorts.Any(portResource => portResource.PortNumber == port);
        }

        private class PortPoolExhaustedException : Exception
        {
        }
    }
}