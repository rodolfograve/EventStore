using System;

namespace EventStore.Core.Exceptions
{
    public class ReaderCheckpointHigherThanWriterException : Exception
    {
        public ReaderCheckpointHigherThanWriterException(string checkpointName)
            : base($"Checkpoint '{checkpointName}' has greater value than writer checkpoint.")
        {
        }
    }
}