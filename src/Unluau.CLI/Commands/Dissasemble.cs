using System.CommandLine;
using Unluau.CLI.Utils;
using Unluau.IR;
using Unluau.IR.Decoders;
using Unluau.IR.Writers;
using Unluau.IR.ControlFlow;

namespace Unluau.CLI.Commands
{
    public class Disassemble : Command
    {
        private static readonly HashSet<string> _supportedFormats = ["ir", "dot", "json"];

        private static readonly Dictionary<string, Decoder> _supportedDecoders = new()
        {
            ["roblox"] = new RobloxDecoder()
        };

        private readonly Option<FileInfo?> _inputOption = new(
            ["-input", "-i" ],
            description: "The input file to disassemble",
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                    return null;

                return new FileInfo(result.Tokens[0].Value);
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

        private readonly Option<bool> _debugFlag = new(
            ["--debug", "-d"],
            description: "Enables debug logging");

        private readonly Option<Decoder?> _decoderOption = new(
            ["--decoder"],
            description: "The decoder to use for the input file",
            parseArgument: result =>
            {
                if (_supportedDecoders.TryGetValue(result.Tokens[0].Value, out var decoder))
                    return decoder;

                result.ErrorMessage = $"Invalid decoder; supported decoders are: {string.Join(", ", _supportedDecoders.Keys)}";
                return null;
            });

        public Disassemble() : base("disassemble", "Disassembles a Luau bytecode file")
        {
            AddOption(_inputOption);
            AddOption(_outputOption);
            AddOption(_formatOption);
            AddOption(_debugFlag);
            AddOption(_decoderOption);

            this.SetHandler(static (inputOpt, outputOpt, formatOpt, debugFlag, decoderOpt) =>
            {
                Stream input = inputOpt?.OpenRead() ?? Console.OpenStandardInput();
                string source = inputOpt?.Name ?? "input-file.luau";
                Stream output = outputOpt ?? Console.OpenStandardOutput();
                string format = formatOpt ?? "ir";
                Decoder decoder = decoderOpt ?? new Decoder();
                var loggerFactory = Logging.CreateLoggerFactory(debugFlag);

                // First we lift the raw bytecode into a high level representation. This representation can be traversed
                // and manipulated easily. This is the first step in the disassembly process.
                var result = Lifter.Lift(input, loggerFactory, source, decoder);

                // Next, we build a control flow graph from the lifted module. This allows us to visualize the flow of
                // the code and analyze it more easily.
                ControlFlowBuilder.Build(loggerFactory, result.Module);

                Writer writer = format switch
                {
                    "ir" => new IRWriter(output),
                    "dot" => new DotWriter(output),
                    "json" => new JsonWriter(output),
                    _ => throw new ArgumentException("Invalid format")
                };

                writer.Write(result);
            }, _inputOption, _outputOption, _formatOption, _debugFlag, _decoderOption);
        }
    }
}
