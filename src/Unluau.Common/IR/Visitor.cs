namespace Unluau.Common.IR
{
    /// <summary>
    /// The base visitor class for the IR instructions. Methods return true if the node's children are 
    /// to be visited as well.
    /// </summary>
    public abstract class Visitor
    {
        /// <summary>
        /// The base visitor for the <see cref="Node"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual bool Visit(Node node) => true;
    }
}
