using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unluau
{
    public class DecompilerOptions
    {
        public bool VariableNameGuessing { get; set; }
        public bool DescriptiveComments { get; set; }
        public bool HeaderEnabled { get; set; } = true;
        public bool InlineTableDefintions { get; set; } = false;
        public string Version { get; set; }
        public Output Output { get; set; } = new Output();
    }

    public class Decompiler
    {
        private DecompilerOptions options;
        private Chunk chunk;

        public Guid Guid { get; private set; }

        public Decompiler(Stream stream, DecompilerOptions options)
        {
            this.options = options;
            chunk = new Deserializer(stream).Deserialize();

            Guid = Guid.NewGuid();
        }

        public void Decompile()
        {
            Lifter lifter = new Lifter(chunk, options);

            if (options.HeaderEnabled)          
                options.Output.WriteLine($"-- Unluau.NET v{options.Version} guid: {Guid}");

            OuterBlock program = lifter.LiftProgram();

            program.Write(options.Output);
        }

        public string Dissasemble()
        {
            return chunk.ToString();
        }
    }
}
