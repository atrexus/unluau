using Unluau.Decompile.IL.Values;

namespace Unluau.Decompile.IL.Statements.Instructions
{
    /// <summary>
    /// Assigns a value to an index.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="index">The index.</param>
    /// <param name="value">The value to assign.</param>
    public class SetIndex(Context context, Values.Index index, BasicValue value) : Instruction(context)
    {
        /// <summary>
        /// The index we are setting.
        /// </summary>
        public Values.Index Index { get; set; } = index;

        /// <summary>
        /// The value we are assigning to the index.
        /// </summary>
        public BasicValue Value { get; set; } = value;

        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Index.Visit(visitor);
                Value.Visit(visitor);
            }
        }
    }
}
