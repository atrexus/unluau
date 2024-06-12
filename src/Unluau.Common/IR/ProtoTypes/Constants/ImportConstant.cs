namespace Unluau.Common.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents an import constant in luau code.
    /// </summary>
    /// <param name="names">The list of string constants that make up the import.</param>
    public class ImportConstant(List<StringConstant> names) : Constant<List<StringConstant>>(ConstantType.Import, names)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);

        /// <inheritdoc/>
        public override string ToString() => $"[{string.Join(",", Value)}]";
    }
}
