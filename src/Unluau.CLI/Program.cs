using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Logging.Abstractions;

namespace Unluau.CLI
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("The command line interface for the Unluau decompiler")
            {
                new Commands.Disassemble()
            };

            return await rootCommand.InvokeAsync(args);
        }
    }

    // Custom Console Formatter for UNIX Timestamp
    public class UnixTimestampConsoleFormatter : ConsoleFormatter
    {
        public UnixTimestampConsoleFormatter() : base("unix") { }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            textWriter.Write($"{unixTimestamp}: {logEntry.Formatter(logEntry.State, logEntry.Exception)}{Environment.NewLine}");
        }
    }
}
