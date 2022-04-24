using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JSecs
{
    class SystemByteGenerator
    {
        private int _systemByte;

        public SystemByteGenerator()
        {
            _systemByte = -1;
            
        }

        public int New() => Interlocked.Increment(ref _systemByte);

    }
}
