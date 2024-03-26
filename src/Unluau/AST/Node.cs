namespace Unluau.AST
{
    /// <summary>
    /// A base node for all AST items.
    /// </summary>
    /// <param name="location">The location of the node.</param>
    public class Node(Location location)
    {
        /// <summary>
        /// The location of the node in the script.
        /// </summary>
        public Location Location { get; set; } = location;

        /// <summary>
        /// Implements the basic visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public virtual void Visit(Visitor visitor)
        {
            return;
        }
    }
}
