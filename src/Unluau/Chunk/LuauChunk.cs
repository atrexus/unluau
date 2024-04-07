using Unluau.Utils;
using Unluau.Chunk.Luau;
using Unluau.IL;
using System.Text;
using Unluau.IL.Statements;

namespace Unluau.Chunk
{
    public class LuauChunk : IChunk
    {
        /// <summary>
        /// Creates a new <see cref="LuauChunk"/> using the given stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>A new chunk.</returns>
        public static IChunk Create(Stream stream)
        {
            var reader = new BinaryReader(stream);

            return new LuauChunk(reader);
        }

        /// <summary>
        /// The version of the current chunk.
        /// </summary>
        public Luau.Version Version { get; private set; }

        /// <summary>
        /// The version number of the type system (null if unsupported).
        /// </summary>
        public byte? TypesVersion { get; private set; }

        /// <summary>
        /// The global symbol table for the chunk (contains names of strings and functions).
        /// </summary>
        public string[] Symbols { get; private set; }

        /// <summary>
        /// The functions located within the main Luau binary chunk.
        /// </summary>
        public Function[] Functions { get; private set; }

        /// <summary>
        /// The index to the main function (the entry-point of the program).
        /// </summary>
        public int MainFunctionIndex { get; private set; }

        /// <summary>
        /// Creates a new instance of a <see cref="LuauChunk"/> using the given binary reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        private LuauChunk(BinaryReader reader)
        {
            Version = new Luau.Version(reader);

            Symbols = ReadSymbolTable(reader);
            Functions = ReadFunctions(reader);

            MainFunctionIndex = reader.ReadSize();
        }

        /// <summary>
        /// Reads the symbol table using the provided binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to use.</param>
        /// <returns>The symbol table.</returns>
        private static string[] ReadSymbolTable(BinaryReader reader)
        {
            var symbolCount = reader.ReadSize();

            var symbols = new string[symbolCount];

            for (int i = 0; i < symbolCount; ++i)
            {
                var symbolSize = reader.ReadSize();

                symbols[i] = reader.ReadASCII(symbolSize);
            }

            return symbols;
        }

        /// <summary>
        /// Reads a list of functions defined using the provided binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to use.</param>
        /// <returns></returns>
        private Function[] ReadFunctions(BinaryReader reader)
        {
            var functionCount = reader.ReadSize();

            var functions = new Function[functionCount];

            for (int i = 0; i < functionCount; ++i)
                functions[i] = new Function(reader, Version, Symbols);

            return functions;
        }

        /// <summary>
        /// Translates the current chunk to the universal IL.
        /// </summary>
        /// <returns>A new IL program.</returns>
        public Program Lift()
        {
            var main = Functions[MainFunctionIndex].Lift(this);
            main.IsMain = true; 

            List<Closure> liftedClosures = [];

            // We want to lift the closures in reversed order, starting with the first functions compiled. This way we 
            // can add upvalues in the lifter without breaking everything.
            for (int id = Functions.Length - 1; id >= 0; --id)
            {
                // Most of the time the main function index is located at 0, but sometimes (for some reason...)
                // the main function is not the first function compiled. 
                if (id == MainFunctionIndex)
                    continue;

                liftedClosures.Add(Functions[id].Lift(this));
            }

            return new Program(main.Context, main, [.. liftedClosures]);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            foreach (var func in Functions)
                stringBuilder.AppendLine(func.ToString() + "\n");

            stringBuilder.AppendLine("main -> " + MainFunctionIndex);

            return stringBuilder.ToString();    
        }
    }
}
