using Microsoft.Extensions.Logging;
using Unluau.IR.ControlFlow.Nodes;

namespace Unluau.IR.ControlFlow.Reductions
{
    /// <summary>
    /// Creates a new instance of the <see cref="SequentialReduction"/> class.
    /// </summary>
    /// <remarks>
    /// Reduces the control flow graph by merging sequential basic blocks together. For this reduction to work, the
    /// following conditions must be met:
    /// - Block 1 must have exactly one successor.
    /// - Block 2 must have less than two successors.
    /// - Block 2's successor cannot be Block 1.
    /// </remarks>
    /// <param name="loggerFactory">The factory to use.</param>
    public class SequentialReduction(ILoggerFactory loggerFactory) : ReductionPass(loggerFactory, "SequentialReduction")
    {
        protected override void Reduce(BasicBlock block)
        {
            if (block.IncomingEdges.Count == 1 && block.OutgoingEdges.Count <= 1)
            {
                var edge = block.IncomingEdges.First();
                var source = edge.Source;

                if (source.OutgoingEdges.Count == 1)
                {
                    _logger.LogDebug("Merging blocks {} and {} into a single sequential block", source, block);

                    // Now we can merge the blocks into one.
                    var sequential = new SequentialBlock(source, block);

                    // Update the incoming and outgoing edges.
                    foreach (var outgoing in block.OutgoingEdges)
                        outgoing.Source = sequential;

                    foreach (var incoming in source.IncomingEdges)
                        incoming.Target = sequential;

                    // Now we locate the two blocks in the protoType's control flow list and replace them with the new block.
                    var index = _protoType!.ControlFlow.IndexOf(source);
                    _protoType.ControlFlow[index] = sequential;
                    _protoType.ControlFlow.Remove(block);
                }
            }
        }
    }
}
