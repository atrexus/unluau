using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace Unluau.CLI
{
    class Program
    {
        private static readonly TextWriter ErrorStream = Console.Error;

        /// <summary>
        /// Avalible options for the Unluau decompiler/dissasembler.
        /// </summary>
        public class Options
        {
            [Option(Default = false, HelpText = "Converts the bytecode to a readable list of instructions.")]
            public bool Dissasemble { get; set; }

            [Option('w', "watermark", Default = false, HelpText = "Displays a watermark comment at the beginning of the decompiled script.")]
            public bool Watermark { get; set; }

            [Option('v', "verbose", Default = false, HelpText = "Adds comments above each statement about the instructions it originated from.")]
            public bool Verbose { get; set; }

            [Option('i', "inline", Default = false, HelpText = "Inlines table definitions.")]
            public bool Inline { get; set; }

            [Option('o', "output", Default = null, HelpText = "The file that the decompiled script will be stored in (stdout otherwise).")]
            public string OutputFile { get; set; }

            [Value(0, MetaName = "input file", HelpText = "Input bytecode file.", Required = true)]
            public string InputFile { get; set; }
        }

        /// <summary>
        /// Entry point for Unluau decompiler and dissasembler
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

            Console.ReadKey();
        }

        static void RunOptions(Options options)
        {
            using (FileStream stream = File.OpenRead(options.InputFile))
            {
                DecompilerOptions decompilerOptions = new DecompilerOptions()
                {
                    Output = options.OutputFile == null ? new Output() : new Output(File.CreateText(options.OutputFile)),
                    DescriptiveComments = options.Verbose,
                    HeaderEnabled = options.Watermark,
                    InlineTableDefintions = options.Inline
                };

                try
                {
                    Decompiler decompiler = new Decompiler(stream, decompilerOptions);

                    if (options.Dissasemble)
                        Console.WriteLine(decompiler.Dissasemble() + "\n\n");

                    decompiler.Decompile();
                }
                catch (DecompilerException e)
                {
                    ErrorStream.WriteLine("UnluauNET -> " + e.Message);
                    Environment.Exit(1);
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            Environment.Exit(1);
        }
    }
}
