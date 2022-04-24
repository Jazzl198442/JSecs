using System;
using System.Collections.Generic;
using System.Text;

namespace JSecs
{
    public class SECSLogFormat
    {
        public bool IsShowBinary { get; set; } = false;
        public bool IsShowCount { get; set; } = false;
        public bool IsShowIndex { get; set; } = false;
        public bool IsShowAttribute { get; set; } = false;
        //public bool IsShowCtrlMsg { get; set; } = false;
        //public bool IsShowTime { get; set; } = true;
        public string DateTimeStringFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
    }
}
