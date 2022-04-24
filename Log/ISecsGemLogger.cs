using System;
using static System.Diagnostics.Trace;

namespace JSecs
{
    public interface ISecsGemLogger
    {
        void MessageIn(SECSMessage msg, SECSLogFormat logConfiguration);
        void MessageOut(SECSMessage msg, SECSLogFormat logConfiguration);

        void Debug(string msg);
        void Info(string msg);
        void Warning(string msg);
        void Error(string msg, Exception ex = null);
    }

    /// <summary>
    /// SECS Connector Logger
    /// </summary>
    public sealed class DefaultSecsGemLogger : ISecsGemLogger
    {
        public void MessageIn(SECSMessage msg, SECSLogFormat logConfiguration)
        {
            //Print log
            try
            {
                var timeStr = DateTime.Now.ToString(logConfiguration.DateTimeStringFormat);
                var priStr = msg.IsPrimary ? "primary" : "secondary";
                var msgBody = msg.ToString(logConfiguration.IsShowCount, logConfiguration.IsShowIndex, logConfiguration.IsShowAttribute);
                WriteLine($"{timeStr} Received {priStr} SECS message, system byte = {msg.SystemByte}, device ID = {msg.Header.SessionID}\r{msgBody}");
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }
        public void MessageOut(SECSMessage msg, SECSLogFormat logConfiguration)
        {
            //Print log
            try
            {
                var timeStr = DateTime.Now.ToString(logConfiguration.DateTimeStringFormat);
                var priStr = msg.IsPrimary ? "primary" : "secondary";
                var msgBody = msg.ToString(logConfiguration.IsShowCount, logConfiguration.IsShowIndex, logConfiguration.IsShowAttribute);
                WriteLine($"{timeStr} Preparing to send {priStr} SECS message, system byte = {msg.SystemByte}, device ID = {msg.Header.SessionID}\r{msgBody}");
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        public void Debug(string msg) => WriteLine(msg);

        public void Info(string msg) => TraceInformation(msg);

        public void Warning(string msg) => TraceWarning(msg);

        public void Error(string msg, Exception ex = null) => TraceError($"{msg}\n {ex}");
    }
}
