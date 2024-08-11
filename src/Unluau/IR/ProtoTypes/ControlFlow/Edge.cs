namespace Unluau.IR.ProtoTypes.ControlFlow
{
    /// <summary>
    /// Represents an edge in the control flow graph.
    /// </summary>
    public class Edge(BasicBlock source, BasicBlock target) : Node
    {
        /// <summary>
        /// The source of the edge.
        /// </summary>
        public BasicBlock Source { get; set; } = source;

        /// <summary>
        /// The target of the edge.
        /// </summary>
        public BasicBlock Target { get; set; } = target;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Source.Accept(visitor);
                Target.Accept(visitor);
            }
        }
    }
}
