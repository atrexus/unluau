namespace Unluau.Decompile.IL.Values.Conditions
{
    /// <summary>
    /// Checks to see if a value is invalid (null or false).
    /// </summary>
    /// <param name="context">Information.</param>
    /// <param name="basicValue">The value to check.</param>
    public class NotTest(Context context, BasicValue basicValue) : BasicCondition(context)
    {
        /// <summary>
        /// The value to check.
        /// </summary>
        public BasicValue Value { get; set; } = basicValue;

        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string? ToString() => $"NotTest({Value})";

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Value.Visit(visitor);
            }
        }
    }
}
