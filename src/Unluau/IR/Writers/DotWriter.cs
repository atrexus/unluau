using System.Text;
using Unluau.IR.ProtoTypes;
using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using Unluau.IR.ProtoTypes.ControlFlow;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.Writers
{
    /// <summary>
    /// Writes the IR to a stream.
    /// </summary>
    public class DotWriter(Stream stream) : Writer(stream)
    {
        private string? _edgeName;
        private int _protoId = 0;

        private DotGraph? _graph;
        private DotSubgraph? _protoSubGraph;
        private StringBuilder? _builder; // used to build basic block contents
        private HashSet<BasicBlock> _blockCache = [];

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
            var labelBuilder = new StringBuilder();

            // Write the function name.
            labelBuilder.Append($"function {(protoType.IsMain ? "main" : (protoType.Name ?? "prototype"))}(");

            // Write the function arguments.
            for (int i = 0; i < protoType.ParameterCount; ++i)
            {
                labelBuilder.Append($"v{i + 1}");

                if (i + 1 < protoType.ParameterCount + (protoType.IsVararg ? 1 : 0))
                    labelBuilder.Append(", ");
            }

            if (protoType.IsVararg)
                labelBuilder.Append("...");

            labelBuilder.Append(')');

            // Create a new subgraph for the function.
            _protoSubGraph = new DotSubgraph()
                .WithIdentifier($"cluster_{_protoId++}")
                .WithLabel(labelBuilder.ToString())
                .WithStyle(DotSubgraphStyle.Filled)
                .WithColor(DotColor.WhiteSmoke)
                .WithAttribute("fontname", "Helvetica");

            _graph!.Add(_protoSubGraph);

            // Visit all basic blocks in the control flow graph.
            protoType.ControlFlow!.Accept(this);

            return false;
        }

        /// <summary>
        /// Writes a basic block to the stream.
        /// </summary>
        /// <param name="block">The block.</param>
        public override bool Visit(BasicBlock block)
        {
            // If the block is empty, we don't need to write it.
            if (_blockCache.Contains(block) || block.Instructions.Count == 0)
                return false;

            _builder = new StringBuilder();

            _builder.Append("<TABLE BORDER=\"0\" CELLSPACING=\"0\" ALIGN=\"LEFT\">");

            // Now we write all of the instructions to the stream.
            foreach (var instruction in block.Instructions)
            {
                _builder.Append("<TR><TD ALIGN=\"LEFT\" VALIGN=\"top\">");

                // Now accept the instruction into the current visitor.
                instruction.Accept(this);

                _builder.Append("</TD></TR>");
            }

            _builder.Append("</TABLE>");

            var basicBlockNode = new DotNode()
                .WithIdentifier($"block_{block.GetHashCode()}")
                .WithLabel(_builder.ToString(), true)
                .WithShape(DotNodeShape.Box)
                .WithAttribute("fontname", "Monospace");

            _protoSubGraph.Add(basicBlockNode);
            _blockCache.Add(block);

            // Accept all outgoing edges.
            block.OutgoingEdges.ForEach(edge => edge.Accept(this));
            return false;
        }

        /// <summary>
        /// Writes an edge to the stream.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public override bool Visit(Edge edge)
        {
            // visit the source and target blocks
            edge.Source.Accept(this);
            edge.Target.Accept(this);

            _edgeName = null;

            if (edge.Source.Branch == BranchType.Can)
            {
                if (edge.Source.OutgoingEdges.Last() == edge)
                    _edgeName = "true";
                else if (edge.Source.OutgoingEdges.First() == edge)
                    _edgeName = "false";
            }

            // Create an edge with identifiers
            var myEdge = new DotEdge()
                .From($"block_{edge.Source.GetHashCode()}").To($"block_{edge.Target.GetHashCode()}")
                .WithArrowHead(DotEdgeArrowType.Vee)
                .WithArrowTail(DotEdgeArrowType.None)
                .WithAttribute("fontname", "Helvetica")
                .WithAttribute("headport", "n")
                .WithAttribute("tailport", "s");

            if (_edgeName != null)
                myEdge = myEdge.WithLabel(_edgeName);

            _protoSubGraph.Add(myEdge);
            return false;
        }

        /// <summary>
        /// Writes an instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(Instruction instruction)
        {
            //var opCode = instruction.Code.ToString().ToLower().PadRight(20);

            _builder!.Append($"{instruction.Code.ToString().ToLower().PadRight(20)}</TD><TD ALIGN=\"LEFT\">");
            return false;
        }

        /// <summary>
        /// Writes an ABC instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionABC instruction)
        {
            Visit(instruction as Instruction);

            _builder!.Append($"{instruction.A} {instruction.B} {instruction.C}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes an AB instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionAB instruction)
        {
            Visit(instruction as Instruction);

            _builder!.Append($"{instruction.A} {instruction.B}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes an AB instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionAD instruction)
        {
            Visit(instruction as Instruction);

            _builder!.Append($"{instruction.A} {instruction.D}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes the auxiliary information for an instruction.
        /// </summary>
        private void WriteAux(Instruction? aux)
        {
            if (aux == null)
                return;

            _builder!.Append("<BR ALIGN=\"LEFT\"/>");

            _builder.Append(aux.Value);
        }

        public override void Write(LiftResult result)
        {
            result.Module.Accept(this);
        }
    }
}
