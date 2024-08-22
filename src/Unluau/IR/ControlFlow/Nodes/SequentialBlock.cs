namespace Unluau.IR.ControlFlow.Nodes
{
    /// <summary>
    /// Represents a sequential block in the control flow.
    /// </summary>
    public class SequentialBlock(BasicBlock first, BasicBlock second) : AbstractBlock
    {
        /// <summary>
        /// The first block in the sequence.
        /// </summary>
        public BasicBlock FirstBlock { get; set; } = first;

        /// <summary>
        /// The second block in the sequence.
        /// </summary>
        public BasicBlock SecondBlock { get; set; } = second;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                FirstBlock.Accept(visitor);
                SecondBlock.Accept(visitor);

                base.Accept(visitor);
            }
        }
    }
}
