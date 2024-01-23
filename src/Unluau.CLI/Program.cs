using Unluau.Chunk;

namespace Unluau.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var stream = File.OpenRead("./test/Print.luau"))
            {
                var chunk = LuauChunk.Create(stream);

                Console.WriteLine("Done");
            }
        }
    }
}
