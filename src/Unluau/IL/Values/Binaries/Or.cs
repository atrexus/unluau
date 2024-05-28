namespace Unluau.Decompile.IL.Values.Binaries
{
    /// <summary>
    /// Represents a logical OR operation in the IL.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="left">The left operand of the operation.</param>
    /// <param name="right">The right operand of the operation.</param>
    public class Or(Context context, BasicValue left, BasicValue right) : BasicBinary(context, left, BinaryType.Or, right)
    {
        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override string? ToString() => $"Or({Left}, {Right})";
    }
}
