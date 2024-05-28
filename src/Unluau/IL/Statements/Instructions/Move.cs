using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Decompile.IL.Statements.Instructions
{
    /// <summary>
    /// Copies a value from one register slot to another. 
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="target">The target register slot.</param>
    /// <param name="source">The source register slot.</param>
    public class Move(Context context, Slot target, Slot source) : Instruction(context)
    {
        /// <summary>
        /// The target register slot.
        /// </summary>
        public Slot Target { get; set; } = target;

        /// <summary>
        /// The source register slot.
        /// </summary>
        public Slot Source { get; set; } = source;

        /// <summary>
        /// Implement visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
