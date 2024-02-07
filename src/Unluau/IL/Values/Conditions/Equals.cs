namespace Unluau.IL.Values.Conditions
{
    /// <summary>
    /// Compares the left and right operands to see if they are equal or not.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="left">Left value.</param>
    /// <param name="right">Right value.</param>
    public class Equals(Context context, Slot left, Slot right) : BasicCondition(context)
    {
        /// <summary>
        /// The left operand in the comparison operation.
        /// </summary>
        public Slot Left { get; set; } = left;

        /// <summary>
        /// The right operand in the comparison operation.
        /// </summary>
        public Slot Right { get; set; } = right;

        /// <summary>
        /// Returns a string representation of the <see cref="Equals"/> instruction.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string? ToString() => $"Eq({Left}, {Right})";

        /// <summary>
        /// Implements the visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
