using Microsoft.Extensions.Logging;
using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ProtoTypes;

namespace Unluau.IR.ControlFlow.Reductions
{
    /// <summary>
    /// The base class for all reduction passes. 
    /// </summary>
    public abstract class ReductionPass(ILoggerFactory loggerFactory, string name) : Visitor
    {
        protected readonly ILogger _logger = loggerFactory.CreateLogger(name);
        protected ProtoType? _protoType;

        /// <summary>
        /// Runs the reduction pass on the given final nodes.
        /// </summary>
        /// <param name="finalNodes">The final nodes to run on.</param>
        /// <param name="protoType">The proto type associated with the pass.</param>
        public void Run(List<BasicBlock> finalNodes, ProtoType protoType)
        {
            _protoType = protoType;

            foreach (var node in finalNodes)
                node.Accept(this);
        }

        /// <summary>
        /// Reduces the given block.
        /// </summary>
        protected abstract void Reduce(BasicBlock block);

        public override bool Visit(BasicBlock block)
        {
            if (_protoType == null)
                throw new InvalidOperationException("The reduction pass has not been initialized.");

            if (_protoType.ControlFlow.Contains(block))
                Reduce(block);

            // We are traversing the tree backwards, so we visit the incoming edges.
            foreach (var edge in block.IncomingEdges)
            {
                edge.Source.Accept(this);
            }

            return false;
        }
    }
}
