using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IL.Instructions;

namespace Unluau.IL
{
    /// <summary>
    /// Represents a basic block in the program.
    /// </summary>
    /// <param name="instructions">A list of instructions.</param>
    public class BasicBlock(Instruction[] instructions) : Node
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
