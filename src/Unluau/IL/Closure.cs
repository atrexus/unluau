using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL
{
    /// <summary>
    /// Provides information on parameters and contents of a closure.
    /// </summary>
    public struct ClosureContext
    { 
        /// <summary>
        /// Basic line and instruction information.
        /// </summary>
        public Context Context { get; set; }
    }

    /// <summary>
    /// Represents a function (closure) within the code.
    /// </summary>
    public class Closure(ClosureContext context, BasicBlock[] blocks) : Node(context.Context)
    {
        /// <summary>
        /// The children blocks of the closure. Each contain
        /// </summary>
        public BasicBlock[] Blocks { get; set; } = blocks;

        /// <summary>
        /// Visits the children of the closure.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var block in Blocks)
                    block.Visit(visitor);
            }
        }
    }
}
