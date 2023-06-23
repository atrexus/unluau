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
    /// Specifies the severity of the log message.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// One or more key business functionalities are not working and the whole system doesn’t fulfill the business functionalities.
        /// </summary>
        Fatal = 0,

        /// <summary>
        /// One or more functionalities are not working, preventing some functionalities from working correctly.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Unexpected behavior happened inside the application, but it is continuing its work and the key business features are operating as expected.
        /// </summary>
        Warn = 2,

        /// <summary>
        /// An event happened, the event is purely informative and can be ignored during normal operations.
        /// </summary>
        Info = 3,

        /// <summary>
        /// A log level used for events considered to be useful during software debugging when more granular information is needed.
        /// </summary>
        Debug = 4,

        /// <summary>
        /// A log level describing events showing step by step execution of your code that can be ignored during the standard operation, but may be useful during extended debugging sessions.
        /// </summary>
        Trace = 5
    }
}
