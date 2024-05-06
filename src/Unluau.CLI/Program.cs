using Unluau.Chunk;
using Unluau.Common.IR;
using Unluau.IL.Visitors;

namespace Unluau.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var stream = File.OpenRead("./test/Types.luau");
            using var reader = new Reader(stream);

            reader.ReadModule();
            
            /*var chunk = LuauChunk.Create(stream);

            Console.WriteLine(chunk.ToString());
            var program = chunk.Lift();

            using var output = Console.OpenStandardOutput();

            program.Visit(new ValueVisitor());
            program.Visit(new OutputVisitor(output));*/
        }
    }
}
