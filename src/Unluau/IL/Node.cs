using Unluau.Common.IR.ProtoTypes.Instructions;
using Unluau.Decompile.IL.Blocks;

namespace Unluau.Decompile.IL
{
    /// <summary>
    /// Contains information about a node in the IL.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the <see cref="Context"/> struct.
    /// </remarks>
    /// <param name="pcs">The range of program counters.</param>
    /// <param name="lines">The range of lines.</param>
    public struct Context(Range pcs, Range lines)
    {
        /// <summary>
        /// The range of program counter values that this node covers.
        /// </summary>
        public Range Pcs { get; set; } = pcs;

        /// <summary>
        /// The range of lines that this node covers.
        /// </summary>
        public Range Lines { get; set; } = lines;

        /// <summary>
        /// Creates a new instance of the <see cref="Context"/> struct.
        /// </summary>
        /// <param name="instruction">The instruction to create the context for.</param>
        public Context(Instruction instruction) : this(new Range(instruction.Context.Pc, instruction.Context.Pc), new Range(instruction.Context.LineDefined ?? 0, instruction.Context.LineDefined ?? 0))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Context"/> struct.
        /// </summary>
        /// <param name="instructions">The list of instructions to build the context from.</param>
        public Context(List<Instruction> instructions) : this(new Range(instructions[0].Context.Pc, instructions[^1].Context.Pc), new Range(instructions[0].Context.LineDefined ?? 0, instructions[^1].Context.LineDefined ?? 0))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Context"/> struct.
        /// </summary>
        /// <param name="controlFlow">The control flow of the program.</param>
        public Context(List<BasicBlock> controlFlow) : this(new Range(controlFlow[0].Context.Pcs.Start, controlFlow[^1].Context.Pcs.Start), new Range(controlFlow[0].Context.Lines.Start, controlFlow[^1].Context.Lines.End))
        {
        }

        /// <summary>
        /// Appends the context of another node to this one.
        /// </summary>
        /// <param name="context">The context of the other node.</param>
        public void Append(Context context)
        {
            if (context.Pcs.End.Value > Pcs.End.Value)
                Pcs = new Range(Pcs.Start, context.Pcs.End);

            if (context.Lines.End.Value > Lines.End.Value)
                Lines = new Range(Lines.Start, context.Lines.End);
        }
    }

    /// <summary>
    /// The base node for all IL components.
    /// </summary>
    /// <param name="context">Additional context on the node.</param>
    public class Node(Context context)
    {
        /// <summary>
        /// Provides additional context on the current node.
        /// </summary>
        public Context Context { get; set; } = context;

        /// <summary>
        /// Recursive visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public virtual void Visit(Visitor visitor)
        {
            return;
        }
    }
}
