using Unluau.IL.Values;

namespace Unluau.IL.Statements.Instructions
{
    /// <summary>
    /// Represents a return instruction in the IL.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="values">List of values to return.</param>
    public class Return(Context context, BasicValue[] values) : Instruction(context)
    {
        /// <summary>
        /// The values the instruction returns.
        /// </summary>
        public BasicValue[] Values { get; set; } = values;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var v in Values)
                    visitor.Visit(v);
            }
        }
    }
}
