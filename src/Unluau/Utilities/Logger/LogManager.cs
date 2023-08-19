// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LogRecievedEventArgs : EventArgs
    {
        public LogMessage Message { get; set; }
    }

    public class LogManager
    {
        private LogSeverity _severity;

        public event EventHandler<LogRecievedEventArgs>? LogRecieved;

        public LogManager(LogSeverity severity)
        {
            _severity = severity;
        }

        public void SendLog(LogSeverity severity, LogRecievedEventArgs args)
        {
            if (_severity >= severity)
                LogRecieved!.Invoke(null, args);
        }
    }
}
