namespace Unluau.Decompile.AST.Expressions
{
    /// <summary>
    /// Represents a basic expression in the AST.
    /// </summary>
    /// <param name="location">The location of the expression in the AST.</param>
    public class Expression(Location location) : Node(location)
    {
        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor) => visitor.Visit(this);
    }
}
