using Unluau.Decompile.Builders;
using Unluau.Decompile.IL.Writers;
using Unluau.Disassemble.Lifting;
using Unluau.Disassemble.Writers;

namespace Unluau.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var output = Console.OpenStandardOutput();


            var lifter = new Lifter(new FileInfo("./test/IfElse.luau"));

            var result = lifter.LiftSource();

            new IRWriter(output).Write(result);

            var program = ILBuilder.Build(result.Module);

            program.Visit(new ILWriter(output));
        }
    }
}
