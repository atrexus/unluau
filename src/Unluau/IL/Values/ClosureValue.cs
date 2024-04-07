namespace Unluau.IL.Values
{
    /// <summary>
    /// Contains a reference to a <see cref="Statements.Closure"/> in the IL.
    /// </summary>
    /// <param name="context">Information about the closure.</param>
    /// <param name="index">The index of the closure in the <see cref="Program"/>.</param>
    public class ClosureValue(Context context, int index) : BasicValue<int>(context, index)
    {
        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns></returns>
        public override string? ToString() => $"CVal({Value})";
    }
}
