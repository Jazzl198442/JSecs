using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs
{
    class InvalidMessageHeaderEventArgs : EventArgs
    {
        public InvalidMessageHeaderEventArgs(byte[] header)
        {
            Header = header;
        }
        public byte[] Header { get; set; }
    }
}
