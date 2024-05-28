namespace Unluau.Decompile.IL.Values.Unaries
{
    /// <summary>
    /// Represents a length operation in the IL.
    /// </summary>
    /// <param name="context">Information on the instruction.</param>
    /// <param name="value">The value to perform the operation on.</param>
    public class Length(Context context, BasicValue value) : BasicUnary(context, UnaryType.Length, value)
    {
        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override string? ToString() => $"Length({Value})";
    }
}
