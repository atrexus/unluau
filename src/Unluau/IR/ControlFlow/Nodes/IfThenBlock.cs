namespace Unluau.IR.ControlFlow.Nodes
{
    /// <summary>
    /// Represents an if-then block in the control flow.
    /// </summary>
    public class IfThenBlock(BasicBlock condition, BasicBlock then) : AbstractBlock
    {
        /// <summary>
        /// The block that contains the jump to the next block.
        /// </summary>
        public BasicBlock ConditionBlock { get; set; } = condition;

        /// <summary>
        /// The block that is executed if the condition is true.
        /// </summary>
        public BasicBlock ThenBlock { get; set; } = then;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                ConditionBlock.Accept(visitor);
                ThenBlock.Accept(visitor);

                base.Accept(visitor);
            }
        }
    }
}
