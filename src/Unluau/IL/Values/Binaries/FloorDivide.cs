namespace Unluau.IL.Values.Binaries
{
    /// <summary>
    /// Represents a floor division operation in the IL.
    /// </summary>
    /// <param name="context">Information on the instruction.</param>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    public class FloorDivide(Context context, BasicValue left, BasicValue right) : BasicBinary(context, left, BinaryType.FloorDivide, right)
    {
        /// <summary>
        /// Returns a string representation of the binary operation.
        /// </summary>
        public override string? ToString() => $"FloorDiv({Left}, {Right})";
    }
}
