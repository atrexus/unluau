using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IR.ControlFlow.Nodes;

namespace Unluau.IR.ControlFlow.Reductions
{
    /// <summary>
    /// Creates a new instance of the <see cref="IfThenReduction"/> class.
    /// </summary>
    /// <remarks>
    /// Reduces the control flow graph by merging if-then constructs into a single block. For this reduction to work, 
    /// the following conditions must be met:
    /// - Block 1 must have two successors.
    /// - Block 2 has only Block 1 as a predecessor.
    /// - Block 2's successor and Block 1's other successor are the same.
    /// - The after block isn't Block 1.
    /// </remarks>
    /// <param name="loggerFactory">The factory to use.</param>
    public class IfThenReduction(ILoggerFactory loggerFactory) : ReductionPass(loggerFactory, "IfThenReduction")
    {
        protected override void Reduce(BasicBlock block)
        {
            // Block 1 must have two successors.
            if (block.OutgoingEdges.Count != 2)
                return;

            var thenBlock = block.OutgoingEdges.First().Target;
            var afterBlock = block.OutgoingEdges.Last().Target;

            // Block 2 has only Block 1 as a predecessor.
            if (thenBlock.IncomingEdges.Count != 1)
                return;

            // Block 2's successor and Block 1's other successor are the same.
            if (thenBlock.OutgoingEdges.Count != 1 || thenBlock.OutgoingEdges.First().Target != afterBlock)
                return;

            // The after block isn't Block 1.
            if (afterBlock == block)
                return;

            _logger.LogDebug("Merging if-then construct ({} and {}) into a single block", block, thenBlock);

            // Now we can merge the blocks into one.
            var ifThen = new IfThenBlock(block, thenBlock);

            // Update the incoming and outgoing edges.
            foreach (var outgoing in thenBlock.OutgoingEdges)
                outgoing.Source = ifThen;

            foreach (var incoming in block.IncomingEdges)
                incoming.Target = ifThen;

            // Now we locate the two blocks in the protoType's control flow list and replace them with the new block.
            var index = _protoType!.ControlFlow.IndexOf(block);
            _protoType.ControlFlow[index] = ifThen;
            _protoType.ControlFlow.Remove(thenBlock);
        }
    }
}
