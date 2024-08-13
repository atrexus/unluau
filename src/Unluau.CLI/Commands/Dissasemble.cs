using System.CommandLine;
using Unluau.IR;
using Unluau.IR.Writers;

namespace Unluau.CLI.Commands
{
    public class Disassemble : Command
    {
        private static readonly HashSet<string> _supportedFormats = ["ir", "dot"];

        private readonly Option<Stream?> _inputOption = new(
            ["-input", "-i" ],
            description: "The input file to disassemble",
            parseArgument: result =>
            {
                return new FileStream(result.Tokens[0].Value, FileMode.Open);
            });

        private readonly Option<Stream> _outputOption = new(
            [ "--output", "-o" ],
            description: "The output file to write the disassembled code to (creates if nonexistent)",
            parseArgument: result =>
            {
                return new FileStream(result.Tokens[0].Value, FileMode.OpenOrCreate);
            });

        private readonly Option<string?> _formatOption = new(
            [ "--format", "-f" ],
            description: "The format to write the disassembled code in",
            parseArgument: result =>
            {
                if (_supportedFormats.Contains(result.Tokens[0].Value))
                    return result.Tokens[0].Value;

                result.ErrorMessage = $"Invalid format; supported formats are: {string.Join(", ", _supportedFormats)}";
                return null;
            });

        public Disassemble() : base("disassemble", "Disassembles a Luau bytecode file")
        {
            AddOption(_inputOption);
            AddOption(_outputOption);
            AddOption(_formatOption);

            this.SetHandler((inputOpt, outputOpt, formatOpt) =>
            {
                Stream input = inputOpt ?? Console.OpenStandardInput();
                Stream output = outputOpt ?? Console.OpenStandardOutput();
                string format = formatOpt ?? "ir";

                var result = new Lifter(input).LiftSource();

                Writer writer = format switch
                {
                    "ir" => new IRWriter(output),
                    "dot" => new DotWriter(output),
                    _ => throw new ArgumentException("Invalid format")
                };

                writer.Write(result);
            }, _inputOption, _outputOption, _formatOption);
        }
    }
}
