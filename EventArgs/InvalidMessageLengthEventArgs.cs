using System;

namespace JSecs
{
    class InvalidMessageLengthEventArgs : EventArgs
    {
        public InvalidMessageLengthEventArgs(int messageLength)
        {
            MessageLength = messageLength;
        }
        public int MessageLength { get; set; }
    }
}
