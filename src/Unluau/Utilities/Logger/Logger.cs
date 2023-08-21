// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Logger
    {

        public string Name { get; }

        private LogManager _manager;

        public Logger(LogManager manager, string name)
        {
            _manager = manager;

            Name = name;
        }

        private void Log(LogSeverity severity, string message, Exception exception = null)
        {
            _manager.SendLog(severity, new LogRecievedEventArgs()
            {
                Message = new LogMessage(severity, Name, message, exception)
            });
        }

        public void Fatal(string message, Exception exception = null)
            => Log(LogSeverity.Fatal, message, exception);

        public void Error(string message, Exception exception = null)
            => Log(LogSeverity.Error, message, exception);

        public void Warning(string message, Exception exception = null)
            => Log(LogSeverity.Warn, message, exception);

        public void Info(string message, Exception exception = null)
            => Log(LogSeverity.Info, message, exception);

        public void Debug(string message, Exception exception = null)
            => Log(LogSeverity.Debug, message, exception);
    }
}
