// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace Unluau.CLI
{
    class Program
    {
        private static readonly TextWriter errorStream = Console.Error;

        private static string Version = "0.0.2-alpha";

        /// <summary>
        /// Avalible options for the Unluau decompiler/dissasembler.
        /// </summary>
        public class Options
        {
            [Option(Default = false, HelpText = "Converts the bytecode to a readable list of instructions.")]
            public bool Dissasemble { get; set; }

            [Option('o', "output", Default = null, HelpText = "The file that the decompiled script will be stored in (stdout otherwise).")]
            public string OutputFile { get; set; }

            [Value(0, MetaName = "input file", Default = null, HelpText = "Input bytecode file (uses stdin if not provided).")]
            public string InputFile { get; set; }

            [Option('v', "verbose", Default = false, HelpText = "Shows log messages as the decompiler is decompiling a script.")]
            public bool Verbose { get; set; }

            #region Decompiler Configuration

            [Option("inline-tables", Default = false, HelpText = "Inlines table definitions. Usually leads to cleaner code.")]
            public bool InlineTables { get; set; }

            [Option("rename-upvalues", Default = true, HelpText = "Renames upvalues to \"upval{x}\" to help distinguish from regular local variables.")]
            public bool RenameUpvalues { get; set; }

            [Option("smart-variable-names", Default = true, HelpText = "Generates logical names for local variables based on their value.")]
            public bool SmartVariableNames { get; set; }

            #endregion
        }

        /// <summary>
        /// Entry point for Unluau decompiler and dissasembler
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            Parser parser = new Parser(with => with.HelpWriter = null);

            ParserResult<Options> result = parser.ParseArguments<Options>(args);

            result.WithParsed(RunOptions);
            result.WithNotParsed(errors => HandleParseError(result, errors));

            // Not required for release builds. 
            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        static void RunOptions(Options options)
        {
            using (Stream stream = string.IsNullOrEmpty(options.InputFile) ? Console.OpenStandardInput() : File.OpenRead(options.InputFile))
            {
                DecompilerOptions decompilerOptions = new DecompilerOptions()
                {
                    Output = options.OutputFile == null ? new Output() : new Output(File.CreateText(options.OutputFile)),
                    DescriptiveComments = options.Verbose,
                    HeaderEnabled = true,
                    InlineTableDefintions = options.InlineTables,
                    RenameUpvalues = options.RenameUpvalues,
                    VariableNameGuessing = options.SmartVariableNames,
                    Version = Version
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
                    errorStream.WriteLine("Unluau -> " + e.Message);
                }

                decompilerOptions.Output.Flush();
            }
        }

        static void HandleParseError<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            HelpText helpText = null;

            if (errors.IsVersion())
                helpText = new HelpText("Unluau " + Version);
            else
            {
                helpText = HelpText.AutoBuild(result, h =>
                {
                    h.Heading = $"Unluau {Version}";
                    h.Copyright = $"Copyright (c) {DateTime.Now.Year} societal";
                    return h;
                }, e => e);
            }

            Console.WriteLine(helpText);
        }
    }
}
