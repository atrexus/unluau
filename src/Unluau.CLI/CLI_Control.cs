using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.CLI
{
    internal class CLI_Control
    {
        private ProgramConfig programConfig;

        public RootCommand rootCommand = new RootCommand("Unluau");
        public void SetupCommands()
        {
            // Output Option
            var option_Output = new Option<string>("--output",
                description: "Define an output file path."
            /*parseArgument: result =>
            {
                return result.Tokens.Single().Value;
            }*/
            );
            option_Output.AddAlias("-o");


            // Disassemble Option
            var option_Disassemble = new Option<bool>("--disassemble",
                description: "This option will print a disassemble version " +
                "of the \"assembled\" Luau machine code to stdout.",
                //isDefault: true,
                parseArgument: result =>
                {
                    this.programConfig.b_Disassemble = true; // Set Config
                    return true;
                }
            )
            { Arity = ArgumentArity.Zero };
            option_Disassemble.AddAlias("-d");

            // Verbose Option
            var option_Verbose = new Option<bool>("--verbose",
                description: "If provided Unluau will enter a verbose mode " +
                "and will display additional information about the decompilation process.",
                parseArgument: result =>
                {
                    this.programConfig.b_Verbose = true;
                    return true;
                }
            )
            { Arity = ArgumentArity.Zero };
            option_Verbose.AddAlias("-v");

            // Supress Warnings Option
            var option_SupressWarnings = new Option<bool>("--supress-warnings",
                description: "If the decompiler is in verbose mode " +
                "and this option is provided, warning logs will not be written to the output stream.",
                parseArgument: result =>
                {
                    this.programConfig.b_SupressWarnings = true;
                    return true;
                }
            )
            { Arity = ArgumentArity.Zero };

            // Logs Option
            var option_Logs = new Option<bool>("--logs",
                description: "This option specifies the output stream for the decompilation logs. " +
                "If this option is not specified then the logs will get printed to standard out, " +
                "otherwise they will be written to the specified file.",
                parseArgument: result =>
                {
                    this.programConfig.b_Logs = true;
                    return true;
                }
            )
            { Arity = ArgumentArity.Zero };

            // Option
            var option_InlineTables = new Option<bool>("--inline-tables",
                description: "Tells the decompiler to inline table definitions.",
                parseArgument: result =>
                {
                    this.programConfig.b_InlineTables = true;
                    return true;
                }
            )
            { Arity = ArgumentArity.Zero };



            // Input File Arg
            var arg_InputFile = new Argument<string>(
                    name: "inputfile.luau",
                    description: "Bytecode input file."
                )
            { Arity = ArgumentArity.ExactlyOne };
            arg_InputFile.AddValidator(result =>
            {
                var inputValue = result.GetValueForArgument(arg_InputFile);

                // Validate if file exists.
                if (!File.Exists(inputValue))
                {
                    // If "ErrorMessage" gets provided
                    // the exit code changes to 1 for InvokeAsync.
                    result.ErrorMessage = $"Provided input file \"{inputValue}\" does not exist.";
                }
            });



            // Add stuff to the RootCommand App.
            rootCommand.AddGlobalOption(option_Output);
            rootCommand.AddGlobalOption(option_Disassemble);
            rootCommand.AddGlobalOption(option_Verbose);
            rootCommand.AddGlobalOption(option_SupressWarnings);
            rootCommand.AddGlobalOption(option_Logs);
            rootCommand.AddGlobalOption(option_InlineTables);

            rootCommand.AddArgument(arg_InputFile);


            this.rootCommand.SetHandler((inputFilePath, outputFilePath) =>
            {
                this.programConfig.inputFilePath = inputFilePath;
                this.programConfig.outputFilePath = outputFilePath;

                return;
            },
                arg_InputFile,
                option_Output
            );
        }


        // Constructor
        public CLI_Control(ProgramConfig ProgramConfig)
        {
            this.programConfig = ProgramConfig;
            this.SetupCommands();
        }
    }
}
