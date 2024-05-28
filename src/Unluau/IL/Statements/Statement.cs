using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Decompile.IL.Statements
{
    /// <summary>
    /// A statement in the IL. These include blocks and instructions.
    /// </summary>
    /// <param name="context">Information about the statement.</param>
    public abstract class Statement(Context context) : Node(context)
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
