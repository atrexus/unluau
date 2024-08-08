using Unluau.Disassemble.Lifting;
using Unluau.Disassemble.Writers;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Collections.Generic;

namespace Unluau.CLI
{
    internal class Program
    {
        private static readonly HashSet<string> _supportedFormats = ["ir"];

        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("The command line interface for the Unluau decompiler");

            var disassembleCommand = new Command("disassemble", "Disassembles a Luau bytecode file")
            {
                new Option<Stream>(
                    ["-input", "-i" ],
                    description: "The input file to disassemble",
                    parseArgument: result =>
                    {
                        if (result.Tokens.Count == 0)
                            return Console.OpenStandardInput();

                        return new FileStream(result.Tokens[0].Value, FileMode.Open);
                    }),
                new Option<Stream>(
                    [ "--output", "-o" ],
                    description: "The output file to write the disassembled code to (creates if nonexistent)",
                    parseArgument: result =>
                    {
                        if (result.Tokens.Count == 0)
                            return Console.OpenStandardOutput();

                        return new FileStream(result.Tokens[0].Value, FileMode.OpenOrCreate);
                    }),
                new Option<string?>(
                    [ "--format", "-f" ],
                    description: "The format to write the disassembled code in",
                    parseArgument: result =>
                    {
                        if (result.Tokens.Count == 0)
                            return "ir";

                        if (_supportedFormats.Contains(result.Tokens[0].Value))
                            return result.Tokens[0].Value;

                        result.ErrorMessage = $"Invalid format; supported formats are: {string.Join(", ", _supportedFormats)}";
                        return null;
                    }),
            };

            disassembleCommand.SetHandler<Stream, Stream, string?>((input, output, format) =>
            {
                var lifter = new Lifter(input);

                lifter.LiftSource();

                var writer = format switch
                {
                    "ir" => new IRWriter(output),
                    _ => throw new ArgumentException("Invalid format")
                };
                writer.Write(result);
            });

            rootCommand.Add(disassembleCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}
