using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IL.Blocks;

namespace Unluau.IL
{
    /// <summary>
    /// The main IL program that contains all of the closures.
    /// </summary>
    public class Program(Context context, BasicBlock[] body, Closure[] closures) : Node(context)
    {
        /// <summary>
        /// The body for the program (main closure).
        /// </summary>
        public BasicBlock[] Body { get; set; } = body;

        /// <summary>
        /// The other functions in the program.
        /// </summary>
        public Closure[] Closures { get; set; } = closures;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var b in Body)
                    b.Visit(visitor);

                foreach (var c in Closures) 
                    c.Visit(visitor);
            }
        }
    }
}
