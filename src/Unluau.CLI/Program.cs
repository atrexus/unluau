using Unluau.Chunk;
using Unluau.IL.Visitors;
using System.CommandLine;
using System.CommandLine.Invocation;

public class ProgramConfig
{
    public string? inputFilePath;
    public string? outputFilePath;

    public bool b_Disassemble = false;
    public string? disassembleFilePath;

    public bool b_Verbose = false;
    public bool b_SupressWarnings = false;
    public bool b_Logs = false;


    // Compiler Specific
    public bool b_InlineTables = false;
}


namespace Unluau.CLI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // The Configuration of the Program
            var ProgramConfig = new ProgramConfig();

            // Process commands
            var CLI = new CLI_Control(ProgramConfig);
            var cliResult = await CLI.rootCommand.InvokeAsync(args);
            if (cliResult == 1 || ProgramConfig.inputFilePath == null) {
                // Terminate if it errored.
                return;
            }


            using var stream = File.OpenRead(ProgramConfig.inputFilePath!);
            var chunk = LuauChunk.Create(stream);

            Console.WriteLine(chunk.ToString());
            var program = chunk.Lift();


            // Incase outputFilePath was provided
            if (ProgramConfig.outputFilePath != null)
            {
                // Currently, unimplemented
            }

            using var output = Console.OpenStandardOutput();

            program.Visit(new ValueVisitor());
            program.Visit(new OutputVisitor(output));
        }
    }
}
