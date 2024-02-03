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
        public List<Instruction> Instructions { get; set; } = new(instructions);

        /// <summary>
        /// Recursive visitor method.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                for (int i = 0; i < instructions.Length; i++)
                    instructions[i].Visit(visitor);
            }
        }
    }
}
