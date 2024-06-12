using Unluau.Decompile.IL.Blocks;
using Unluau.Decompile.IL.Instructions;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using DotNetGraph.Compilation;
using System.Text;
using DotNetGraph.Attributes;
using Unluau.Decompile.AST.Statements;
using Unluau.Decompile.IL.Values;

namespace Unluau.Decompile.IL.Writers
{
    /// <summary>
    /// Writes the IL program to a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public class ILWriter(Stream stream) : Writer(stream)
    {
        private readonly DotGraph _dotGraph = new DotGraph()
            .WithIdentifier("unluau IL")
            .Directed()
            .WithRankDir(DotRankDir.TB);

        private readonly StringBuilder _builder = new();
        private readonly List<DotSubgraph> _dotSubGraphs = [];

        public override bool Visit(Program program)
        {
            // Visit the children of the program
            foreach (var function in program.Functions)
                function.Visit(this);

            foreach (var subGraph in _dotSubGraphs)
                _dotGraph.Add(subGraph);

            var context = new CompilationContext(_writer, new CompilationOptions() { AutomaticEscapedCharactersFormat = false });

            _dotGraph.CompileAsync(context).Wait();

            return false;
        }

        public override bool Visit(Function function)
        {
            var name = function.IsEntryPoint ? "__main__" : function.Name;
            var subGraph = new DotSubgraph()
                .WithIdentifier(name);

            var main = new DotNode()
                .WithIdentifier(name)
                .WithShape(DotNodeShape.Oval)
                .WithLabel($"__main__");

            subGraph.Add(main);

            _dotSubGraphs.Add(subGraph);

            function.VisitChildren(this);

            subGraph.Add(new DotEdge()
                .From(main)
                .To(function.ControlFlow.GetHashCode().ToString()));

            return false;
        }

        public override bool Visit(BasicBlock block)
        {
            var lastSubGraph = _dotSubGraphs.Last();

            foreach (var instruction in block)
            {
                instruction.Visit(this);
                _builder.Append('\\');
                _builder.Append('l');
            }

            var node = new DotNode()
                .WithIdentifier(block.GetHashCode().ToString())
                .WithAttribute("nojustify", "true")
                .WithLabel(_builder.ToString().Replace("\"", "\\\""))
                .WithShape(DotNodeShape.Rect)
                .WithAttribute("fontname", new DotNodeStyleAttribute("Consolas"));

            lastSubGraph.Add(node);

            if (block.Next != null)
            {
                block.Next.Visit(this);

                lastSubGraph.Add(new DotEdge()
                    .From(node)
                    .To(block.Next.GetHashCode().ToString()));
            }

            return false;
        }

        public override bool Visit(LoadValue instruction)
        {
            _builder.Append($"load {instruction.Slot} {instruction.Value}");
            return false;
        }

        public override bool Visit(Call instruction)
        {
            _builder.Append($"call {instruction.CallResult}");
            return false;
        }

        public override bool Visit(Return instruction)
        {
            _builder.Append($"return {BasicValue.ToString(instruction.Values)}");
            return false;
        }
    }
}
