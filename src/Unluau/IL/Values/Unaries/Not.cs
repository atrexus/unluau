namespace Unluau.Decompile.IL.Values.Unaries
{
    /// <summary>
    /// Represents a logical NOT operation in the IL.
    /// </summary>
    /// <param name="context">Information on the instruction.</param>
    /// <param name="value">The value to perform the operation on.</param>
    public class Not(Context context, BasicValue value) : BasicUnary(context, UnaryType.Not, value)
    {
        /// <summary>
        /// Returns a string representation of the <see cref="Not"/> instruction.
        /// </summary>
        public override string? ToString() => $"Not({Value})";
    }
}
