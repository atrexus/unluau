using Unluau.IL.Values;

namespace Unluau.IL.Statements.Instructions
{
    /// <summary>
    /// Assigns a global to a value.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="global">The global.</param>
    /// <param name="value">The value to assign.</param>
    public class SetGlobal(Context context, Global global, BasicValue value) : Instruction(context)
    {
        /// <summary>
        /// The global we are setting.
        /// </summary>
        public Global Global { get; set; } = global;

        /// <summary>
        /// The value we are setting the global to.
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
                Global.Visit(visitor);
                Value.Visit(visitor);
            }
        }
    }
}
