// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    /// <summary>
    /// Message object used in logs
    /// </summary>
    public struct LogMessage
    {
        /// <summary>
        /// Gets the severity of the log message
        /// </summary>
        /// <returns>
        /// A <see cref="LogSeverity"/> enum to indicate the severeness of the event.
        /// </returns>
        public LogSeverity Severity { get; }

        /// <summary>
        /// The source of the log message (where it was fired).
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The exception that this message originates from.
        /// </summary>
        public Exception Exception { get; }

        public LogMessage(LogSeverity severity, string source, string message, Exception exception = null)
        {
            Severity = severity;
            Source = source;
            Message = message;
            Exception = exception;
        }
    }
}
