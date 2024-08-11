namespace Unluau.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents a closure in Luau code (functions).
    /// </summary>
    /// <param name="index">The index of the associated function prototype in the Luau bytecode.</param>
    public class ClosureConstant(int index) : Constant<int>(ConstantType.Closure, index)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }
}
