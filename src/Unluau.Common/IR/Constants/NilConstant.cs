namespace Unluau.Common.IR.Constants
{
    /// <summary>
    /// Constant representing the `nil` keyword.
    /// </summary>
    public class NilConstant() : Constant(ConstantType.Nil)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);

        /// <inheritdoc/>
        public override string? ToString() => "nil";
    }
}
