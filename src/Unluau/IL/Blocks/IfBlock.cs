using Unluau.IL.Instructions;
using Unluau.IL.Values.Conditions;

namespace Unluau.IL.Blocks
{
    /// <summary>
    /// A block of code that gets executed if the condition succeeds.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="instructions">The code.</param>
    public class IfBlock(Context context, BasicCondition condition, List<Instruction> instructions) : BasicBlock(context, instructions)
    {
        /// <summary>
        /// The condition that the block relies on.
        /// </summary>
        public BasicCondition Condition { get; set; } = condition;

        /// <summary>
        /// A block of code that gets executed if the condition succeeds.
        /// </summary>
        /// <param name="context">Information about the instruction.</param>
        /// <param name="basicCondition">The condition.</param>
        public IfBlock(Context context, BasicCondition basicCondition) : this(context, basicCondition, [])
        { 
        }

        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Condition.Visit(visitor);
            }
        }
    }
}
