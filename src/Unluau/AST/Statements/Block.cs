namespace Unluau.Decompile.AST.Statements
{
    /// <summary>
    /// A block of statements.
    /// </summary>
    /// <param name="location">The location of the block in the script.</param>
    /// <param name="body">The body of the block.</param>
    public class Block(Location location, List<Statement> body) : Statement(location)
    {
        /// <summary>
        /// Body of the statements.
        /// </summary>
        public List<Statement> Body { get; set; } = body;

        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var statment in Body.ToList())
                    visitor.Visit(statment);
            }
        }
    }
}
