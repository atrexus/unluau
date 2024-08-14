using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.CLI.Utils
{
    public class Logging
    {
        public static ILoggerFactory CreateLoggerFactory(bool debug)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole().AddConsole();
                builder.SetMinimumLevel(debug ? LogLevel.Debug : LogLevel.Information);
            });

            return loggerFactory;
        }
    }
}

