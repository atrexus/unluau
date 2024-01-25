using Unluau.IL.Instructions;

namespace Unluau.IL.Blocks
{
    /// <summary>
    /// Represents a basic block in the program.
    /// </summary>
    /// <param name="context">Provides context about the block.</param>
    /// <param name="instructions">A list of instructions.</param>
    public class BasicBlock(Context context, Instruction[] instructions) : Node(context)
    {
        /// <summary>
        /// The instructions within the block.
        /// </summary>
        public Instruction[] Instructions { get; set; } = instructions;

        /// <summary>
        /// Recursive visitor method.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (Instruction instruction in Instructions)
                    instruction.Visit(visitor);
            }
        }
    }
}
