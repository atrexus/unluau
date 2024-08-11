namespace Unluau.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents a table constant in luau code.
    /// </summary>
    /// <param name="keys">List of constants that make up the keys in the table.</param>
    public class TableConstant(List<Constant> keys) : Constant<List<Constant>>(ConstantType.Import, keys)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);

        /// <inheritdoc/>
        public override string ToString() => $"{{{string.Join(", ", Value)}}}";
    }
}
