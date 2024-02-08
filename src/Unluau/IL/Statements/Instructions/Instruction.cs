using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL.Statements.Instructions
{
    /// <summary>
    /// Creates a new instance of <see cref="Instruction"/>.
    /// </summary>
    /// <param name="context">Provides context about the instruction.</param>
    public abstract class Instruction(Context context) : Statement(context)
    {
        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
