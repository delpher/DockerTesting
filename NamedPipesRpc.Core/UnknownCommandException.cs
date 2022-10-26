using System;

namespace NamedPipesRpc.Core
{
    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string name) : base($"Command '{name}' is not recognized.")
        {
        }
    }
}