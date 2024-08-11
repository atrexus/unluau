namespace Unluau.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents a boolean constant.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    public class BooleanConstant(bool value) : Constant<bool>(ConstantType.Boolean, value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);

        /// <inheritdoc/>
        public override string? ToString() => Value ? "true" : "false";
    }
}
