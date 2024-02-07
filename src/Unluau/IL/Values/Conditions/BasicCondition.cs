namespace Unluau.IL.Values.Conditions
{
    /// <summary>
    /// A condition in the IL. These include binary as well as unary comparisons.
    /// </summary>
    /// <param name="context">Information about the comparison.</param>
    public abstract class BasicCondition(Context context) : BasicValue(context)
    {
        /// <summary>
        /// Creates a string representation of the <see cref="BasicCondition"/>.
        /// </summary>
        /// <returns>String representation.</returns>
        public override abstract string? ToString();
    }
}
