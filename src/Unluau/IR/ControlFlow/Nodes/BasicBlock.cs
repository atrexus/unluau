namespace Unluau.IR.ControlFlow.Nodes
{
    /// <summary>
    /// Represents a basic block in the control flow. Its the most basic unit of control flow.
    /// </summary>
    public class BasicBlock : Node
    {
        /// <summary>
        /// Gets the unique identifier for this block.
        /// </summary>
        public int Id { get => GetHashCode(); }

        /// <summary>
        /// The list of edges that are outgoing from the current block.
        /// </summary>
        public List<Edge> OutgoingEdges { get; set; } = [];

        /// <summary>
        /// The list of edges that are incoming to the current block.
        /// </summary>
        public List<Edge> IncomingEdges { get; set; } = [];

        /// <summary>
        /// Adds an edge to the current block.
        /// </summary>
        /// <param name="target">The target block.</param>
        public void AddEdge(BasicBlock target, string? label = null)
        { 
            var edge = new Edge(this, target, label);

            OutgoingEdges.Add(edge);
            target.IncomingEdges.Add(edge);
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var edge in OutgoingEdges)
                    edge.Accept(visitor);

                foreach (var edge in IncomingEdges)
                    edge.Accept(visitor);
            }
        }

        /// <inheritdoc/>
        public override string ToString() => Id.ToString("x");
    }
}
