namespace Unluau.Common.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents a number constant.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    public class NumberConstant(double value) : Constant<double>(ConstantType.Number, value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }
}
