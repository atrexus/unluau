namespace Unluau.IL.Values.Unaries
{
    /// <summary>
    /// Represents a minus operation in the IL.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="value">The value to perform the operation on.</param>
    public class Minus(Context context, BasicValue value) : BasicUnary(context, UnaryType.Minus, value)
    {
        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override string? ToString() => $"Minus({Value})";
    }
}
