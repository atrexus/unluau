using System.CommandLine;
using System.CommandLine.Parsing;

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
}
