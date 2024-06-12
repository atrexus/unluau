using System.Numerics;

namespace Unluau.Common.IR.ProtoTypes.Constants
{
    /// <summary>
    /// Represents a vector constant in Luau bytecode.
    /// </summary>
    public class VectorConstant(float x, float y, float z, float w) : Constant<Vector4>(ConstantType.Vector, new(x, y, z, w))
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }
}
