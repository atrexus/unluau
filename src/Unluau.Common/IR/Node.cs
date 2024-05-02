namespace Unluau.Common.IR
{
    /// <summary>
    /// A base IR node for all items in the IR.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Accepts a visitor into the current node.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(Visitor visitor);
    }
}
