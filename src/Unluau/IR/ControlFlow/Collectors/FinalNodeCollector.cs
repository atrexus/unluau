using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ProtoTypes;

namespace Unluau.IR.ControlFlow.Collectors
{
    /// <summary>
    /// Class responsible for collecting the final nodes of a control flow graph.
    /// </summary>
    public class FinalNodeCollector : Visitor
    {
        private readonly HashSet<BasicBlock> _finalNodes = [];
        private readonly HashSet<BasicBlock> _blockCache = []; // Cache to avoid visiting the same block multiple times.

        /// <summary>
        /// Collects the final nodes of the control flow graph in the given ProtoType.
        /// </summary>
        /// <param name="protoType">The proto type.</param>
        /// <returns>A list of blocks. </returns>
        public static List<BasicBlock> Collect(ProtoType protoType)
        {
            var collector = new FinalNodeCollector();
            protoType.Accept(collector);

            return [.. collector._finalNodes];
        }

        public override bool Visit(BasicBlock block)
        {
            if (_blockCache.Contains(block))
                return false;

            _blockCache.Add(block);

            if (block.OutgoingEdges.Count == 0)
                _finalNodes.Add(block);

            return true;
        }
    }
}
