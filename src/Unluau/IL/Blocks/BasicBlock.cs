using System.Collections;
using Unluau.Decompile.IL.Instructions;

namespace Unluau.Decompile.IL.Blocks
{
    /// <summary>
    /// Represents a basic block of code in the program.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="BasicBlock"/>.
    /// </remarks>
    public class BasicBlock() : Node(new()), ICollection<Instruction>
    {
        private readonly List<Instruction> _instructions = [];

        /// <inheritdoc/>
        public int Count => _instructions.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// The next block in the control flow.
        /// </summary>
        public BasicBlock? Next { get; set; }

        /// <inheritdoc/>
        public void Add(Instruction item)
        {
            _instructions.Add(item);

            Context.Append(item.Context);
        }

        /// <inheritdoc/>
        public void Clear() => _instructions.Clear();

        /// <inheritdoc/>
        public bool Contains(Instruction item) => _instructions.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(Instruction[] array, int arrayIndex) => _instructions.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<Instruction> GetEnumerator() => _instructions.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _instructions.GetEnumerator();

        /// <inheritdoc/>
        public bool Remove(Instruction item) => _instructions.Remove(item);

        /// <summary>
        /// Visits the children of the block.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void VisitChildren(Visitor visitor)
        {
            foreach (var instruction in _instructions)
                instruction.Visit(visitor);
        }

        /// <inheritdoc/>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                VisitChildren(visitor);
                Next?.Visit(visitor);
            }
        }
    }
}
