using Unluau.Disassembler;

namespace Unluau.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var reader = new Lifter(new FileInfo("./test/IfElse.luau"));

            var module = reader.LiftModule();
            Writer.WriteTo(Console.OpenStandardOutput(), module);
            //IRWriter.WriteTo(Console.OpenStandardOutput(), module);
               
/*            Console.WriteLine(chunk.ToString());
            var program = chunk.Lift();

            using var output = Console.OpenStandardOutput();

            program.Visit(new ValueVisitor());
            program.Visit(new OutputVisitor(output));*/
        }
    }
}
