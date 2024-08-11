using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ProtoTypes.ControlFlow
{
    /// <summary>
    /// Represents the control flow graph of the IR. It consists of a sequence of blocks that are connected by edges.
    /// </summary>
    public class Graph : Node
    {
        private readonly Queue<(int, BasicBlock)> _edgeQueue = new();
        private readonly BasicBlock _entryBlock;
        private BasicBlock _currentBlock;

        /// <summary>
        /// The count of blocks in the graph.
        /// </summary>
        public int BlockCount { get; private set; }

        /// <summary>
        /// The count of instructions in the graph.
        /// </summary>
        public int InstructionCount { get; private set; }

        /// <summary>
        /// A list of all the blocks in the graph.
        /// </summary>
        public HashSet<BasicBlock> Blocks { get; } = [];

        /// <summary>
        /// Creates a new control flow graph.
        /// </summary>
        public Graph()
        {
            _entryBlock = new BasicBlock();
            _currentBlock = _entryBlock;
        }

        /// <summary>
        /// Adds an instruction to the current block. Handles the branching logic.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public void AddInstruction(Instruction instruction)
        {
            // check if we have any edges to add
            if (_edgeQueue.TryPeek(out var edge) && edge.Item1 == InstructionCount)
            {
                _edgeQueue.Dequeue();

                edge.Item2.OutgoingEdges.Add(new Edge(_currentBlock, edge.Item2));
            }

            _currentBlock.Instructions.Add(instruction);
            
            // save the current block for branching
            var currentBlock = _currentBlock;

            switch (instruction.Code)
            {
                case OpCode.JumpBack:
                case OpCode.Jump:
                {
                    currentBlock.Branch = BranchType.Always;

                    _edgeQueue.Enqueue((InstructionCount + instruction.D, currentBlock));

                    _currentBlock = new BasicBlock();
                    break;
                }
                case OpCode.Return:
                {
                    currentBlock.Branch = BranchType.Always;

                    _currentBlock = new BasicBlock();
                    break;
                }
                case OpCode.JumpIfNotEq:
                {
                    currentBlock.Branch = BranchType.Can;

                    _edgeQueue.Enqueue((InstructionCount + instruction.D, currentBlock));

                    _currentBlock = new BasicBlock();
                    currentBlock.OutgoingEdges.Add(new Edge(currentBlock, _currentBlock));
                    break;
                }

            }

            Blocks.Add(_currentBlock); // will fail if the block is already in the set
            InstructionCount++;
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                _entryBlock.Accept(visitor);
            }
        }
    }
}
