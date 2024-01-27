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
    public class Program(Context context, Closure main, Closure[] closures) : Node(context)
    {
        /// <summary>
        /// The main closure.
        /// </summary>
        public Closure Main { get; set; } = main;

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
                Main.Visit(visitor);

                foreach (var c in Closures) 
                    c.Visit(visitor);
            }
        }
    }
}
