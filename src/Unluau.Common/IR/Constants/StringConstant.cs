namespace Unluau.Common.IR.Constants
{
    /// <summary>
    /// Represents a string constant.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    public class StringConstant(string value) : Constant<string>(ConstantType.String, value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);

        /// <inheritdoc/>
        public override string ToString() => $"\"{Value}\"";
    }
}
