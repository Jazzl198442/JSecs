using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs
{
    class RejectEventArgs : EventArgs
    {
        public RejectEventArgs(SECSMessageHeader reqHeader, byte reasonCode, string message)
        {
            RequestHeader = reqHeader;
            ReasonCode = reasonCode;
            Message = message;
        }

        public SECSMessageHeader RequestHeader { get; }
        public byte ReasonCode { get; }
        public string Message { get; }
    }
}
