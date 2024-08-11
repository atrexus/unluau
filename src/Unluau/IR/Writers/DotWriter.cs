using System.Text;
using Unluau.IR.ProtoTypes.ControlFlow;
using Unluau.IR.ProtoTypes.Instructions;
using Unluau.IR.ProtoTypes;
using Unluau.IR.Versions;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;

namespace Unluau.IR.Writers
{
    /// <summary>
    /// Writes the IR to a stream.
    /// </summary>
    public class DotWriter(Stream stream) : Writer(stream)
    {
        private const string _tab = "   ";
        private int _pc = 0;

        private DotGraph? _graph;

        /// <summary>
        /// Writes the main module to the stream.
        /// </summary>
        /// <param name="module">The module.</param>
        public override bool Visit(Module module)
        {
            _graph = new DotGraph().WithIdentifier(module.Checksum.Source).Directed();

            // Visit all function prototypes in the module.
            module.ProtoTypes.ForEach(proto => proto.Accept(this));

            var context = new CompilationContext(_writer, new());
            _graph.CompileAsync(context).Wait();
            return false;
        }

        /// <summary>
        /// Writes a function prototype to the stream.
        /// </summary>
        /// <param name="protoType">The prototype.</param>
        public override bool Visit(ProtoType protoType)
        {

            return false;
        }

        public override void Write(LiftResult result)
        {
            result.Module.Accept(this);
        }
    }
}
