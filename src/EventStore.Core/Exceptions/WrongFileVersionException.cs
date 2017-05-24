using System;

namespace EventStore.Core.Exceptions
{
    public class WrongFileVersionException : Exception
    {
        public WrongFileVersionException(string filename, byte fileVersion, byte expectedVersion)
                : base($"File {filename} has wrong version: {fileVersion}, while expected version is: {expectedVersion}.")
        {
        }
    }
}