using Unluau.IL.Instructions;
using Unluau.IL.Values;

namespace Unluau.IL.Blocks
{
    /// <summary>
    /// Represents a basic block in the program.
    /// </summary>
    /// <param name="context">Provides context about the block.</param>
    /// <param name="instructions">A list of instructions.</param>
    public class BasicBlock(Context context, List<Instruction> instructions) : Node(context)
    {
        /// <summary>
        /// The instructions within the block.
        /// </summary>
        public List<Instruction> Instructions { get; set; } = instructions;

        /// <summary>
        /// Creates a new <see cref="BasicBlock"/>.
        /// </summary>
        /// <param name="context">Context about the block.</param>
        public BasicBlock(Context context) : this(context, [])
        {

        }

        /// <summary>
        /// Recursive visitor method.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var instruction in Instructions)
                    instruction.Visit(visitor);
            }
        }
    }
}
