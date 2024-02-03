using Unluau.IL.Blocks;
using Unluau.IL.Values;

namespace Unluau.IL
{
    /// <summary>
    /// Provides information on parameters and contents of a closure.
    /// </summary>
    public struct ClosureContext
    {
        /// <summary>
        /// A list of variables that act as parameters to the closure.
        /// </summary>
        public Variable[] Parameters { get; set; }

        /// <summary>
        /// Whether or not the function is variadic (includes `...`).
        /// </summary>
        public bool IsVariadic { get; set; }

        /// <summary>
        /// Basic line and instruction information.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// The symbol name assigned to this closure.
        /// </summary>
        public string? Symbol { get; set; }  
    }

    /// <summary>
    /// Represents a function (closure) within the code.
    /// </summary>
    public class Closure(ClosureContext context, BasicBlock[] blocks) : Node(context.Context)
    {
        /// <summary>
        /// A list of variables that act as parameters to the closure.
        /// </summary>
        public Variable[] Parameters { get; set; } = context.Parameters;

        /// <summary>
        /// Whether or not the function is variadic (includes `...`).
        /// </summary>
        public bool IsVariadic { get; set; } = context.IsVariadic;

        /// <summary>
        /// The children blocks of the closure. Each contain
        /// </summary>
        public BasicBlock[] Blocks { get; set; } = blocks;

        /// <summary>
        /// The name of the current closure.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Whether or not this is the main closure (entrypoint).
        /// </summary>
        public bool IsMain { get; set; } = false;

        /// <summary>
        /// Visits the children of the closure.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void VisitChildren(Visitor visitor)
        {
            foreach (var block in Blocks)
                block.Visit(visitor);
        }

        /// <summary>
        /// Implements the visitor.
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
