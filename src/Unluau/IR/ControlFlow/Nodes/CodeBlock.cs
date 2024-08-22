using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ControlFlow.Nodes
{
    /// <summary>
    /// The type of branch that is taken.
    /// </summary>
    public enum BranchType
    {
        /// <summary>
        /// Two blocks are associated, one for the true branch and one for the false branch.
        /// </summary>
        Can,

        /// <summary>
        /// The block always branches.
        /// </summary>
        Always,

        /// <summary>
        /// This block never branches.
        /// </summary>
        Never,
    }

    /// <summary>
    /// Represents a code block in the control flow. It consists of a sequence of instructions.
    /// </summary>
    public class CodeBlock : BasicBlock
    {
        /// <summary>
        /// The kind of branch that is taken.
        /// </summary>
        public BranchType? Branch { get; set; }

        /// <summary>
        /// The list of instructions in the basic block.
        /// </summary>
        public List<Instruction> Instructions { get; set; } = [];

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var instruction in Instructions)
                    instruction.Accept(visitor);

                base.Accept(visitor);
            }
        }
    }
}
