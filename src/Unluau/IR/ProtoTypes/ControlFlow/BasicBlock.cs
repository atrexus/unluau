using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ProtoTypes.ControlFlow
{
    /// <summary>
    /// The type of branch that is taken.
    /// </summary>
    public enum BranchType
    {
        /// <summary>
        /// Two blocks are associated, one for the true branch and one for the false branch.
        /// </summary>
        Can,

        /// <summary>
        /// The block always branches.
        /// </summary>
        Always,

        /// <summary>
        /// This block never branches.
        /// </summary>
        Never,
    }

    /// <summary>
    /// Represents a basic block in the control flow. It consists of a sequence of instructions that are executed in order.
    /// </summary>
    public class BasicBlock : Node
    {
        /// <summary>
        /// The kind of branch that is taken.
        /// </summary>
        public BranchType? Branch { get; set; }

        /// <summary>
        /// The list of instructions in the basic block.
        /// </summary>
        public List<Instruction> Instructions { get; set; } = [];

        /// <summary>
        /// The list of edges that are outgoing from the current block.
        /// </summary>
        public List<Edge> OutgoingEdges { get; set; } = [];

        /// <summary>
        /// Adds an edge to the current block.
        /// </summary>
        /// <param name="target">The target block.</param>
        public void AddEdge(BasicBlock target, string? label = null) => OutgoingEdges.Add(new Edge(this, target, label));

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var instruction in Instructions)
                    instruction.Accept(visitor);

                foreach (var edge in OutgoingEdges)
                    edge.Accept(visitor);
            }
        }
    }
}
