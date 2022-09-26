using System;
using System.IO;

namespace Unluau.CLI
{
    class Program
    {
        /// <summary>
        /// Entry point for the Luau Deompiler
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            string directory = "C:\\Users\\max\\source\\repos\\LuauCompiler\\LuauCompiler\\scripts";

            using (FileStream stream = File.OpenRead(directory + "\\print.bin"))
            {
                Decompiler decompiler = new Decompiler(stream, new DecompilerOptions() { InlineTableDefintions = true });

                Console.WriteLine(decompiler.Dissasemble() + "\n\n");
                decompiler.Decompile();
            }

            Console.ReadKey();
        }
    }
}
