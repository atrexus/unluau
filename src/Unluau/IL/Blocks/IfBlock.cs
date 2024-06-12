using Unluau.Decompile.IL.Conditions;

namespace Unluau.Decompile.IL.Blocks
{
    /// <summary>
    /// A block that represents an if statement.
    /// </summary>
    public class IfBlock : BasicBlock
    {
        /// <summary>
        /// Creates a new instance of <see cref="IfBlock"/>.
        /// </summary>
        /// <param name="condition">The condition the block relies on.</param>
        public IfBlock(BasicCondition condition)
        {
            Condition = condition;
            Context.Append(condition.Context);
        }

        /// <summary>
        /// The condition the block relies on.
        /// </summary>
        public BasicCondition Condition { get; set; }
        
        /// <inheritdoc/>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
                Condition.Visit(visitor);
        }
    }
}
