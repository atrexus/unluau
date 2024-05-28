namespace Unluau.Decompile.IL.Values.Conditions
{
    /// <summary>
    /// Condition that checks if the basic value is valid (not null or true boolean).
    /// </summary>
    /// <param name="context"></param>
    /// <param name="basicValue"></param>
    public class Test(Context context, BasicValue basicValue) : BasicCondition(context)
    {
        /// <summary>
        /// The value to check.
        /// </summary>
        public BasicValue Value { get; set; } = basicValue;

        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string? ToString() => $"Test({Value})";

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
