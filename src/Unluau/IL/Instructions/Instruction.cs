namespace Unluau.Decompile.IL.Instructions
{
    /// <summary>
    /// Creates a new instance of <see cref="Instruction"/>.
    /// </summary>
    /// <param name="context">Provides context about the instruction.</param>
    public abstract class Instruction(Context context) : Node(context)
    {
        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
