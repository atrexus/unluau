namespace Unluau.Decompile.AST.Statements
{
    /// <summary>
    /// Represents a basic statement in the AST.
    /// </summary>
    /// <param name="location">Location of the statement in the script.</param>
    public class Statement(Location location) : Node(location)
    {
        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor) => visitor.Visit(this);
    }
}
