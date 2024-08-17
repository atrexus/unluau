using Unluau.IR;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ControlFlow
{
    /// <summary>
    /// Represents the control flow graph of the IR. It consists of a sequence of blocks that are connected by edges.
    /// </summary>
    public class Graph : Node
    {
        private readonly List<(int, BasicBlock, string?)> _edgeQueue = [];
        private readonly BasicBlock _entryBlock;
        private BasicBlock _currentBlock;

        /// <summary>
        /// The count of blocks in the graph.
        /// </summary>
        public int BlockCount { get; private set; }

        /// <summary>
        /// A list of all the blocks in the graph.
        /// </summary>
        public HashSet<BasicBlock> Blocks { get; } = [];

        /// <summary>
        /// Creates a new control flow graph.
        /// </summary>
        public Graph(List<Instruction> instructions)
        {
            _entryBlock = new BasicBlock();
            _currentBlock = _entryBlock;

            foreach (var instruction in instructions)
                AddInstruction(instruction);
        }

        /// <summary>
        /// Adds an instruction to the current block. Handles the branching logic.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void AddInstruction(Instruction instruction)
        {
            var oldBlock = _currentBlock;

            // Check if there are any edges that need to be added to the current block.
            for (var i = 0; i < _edgeQueue.Count; i++)
            {
                var (pc, block, label) = _edgeQueue[i];

                if (instruction.Context.Pc == pc + 1)
                {
                    // We only want to create a new block if this is the first edge that we are adding.
                    if (_currentBlock == oldBlock && _currentBlock.Instructions.Count > 0)
                        _currentBlock = new();

                    block.AddEdge(_currentBlock, label);
                    _edgeQueue.RemoveAt(i);
                    i--;
                }
            }

            _currentBlock.Instructions.Add(instruction);

            // If we have created a new block, and the old one has no branches, we need to add an edge to the new block.
            // This way this block will be reachable from the old block.
            if (_currentBlock != oldBlock && oldBlock.Branch is null)
                oldBlock.AddEdge(_currentBlock);

            switch (instruction.Code)
            {
                case OpCode.JumpX:
                    {
                        AddEdge(instruction.E, instruction.Context.Pc, _currentBlock, BranchType.Always);
                        break;
                    }
                case OpCode.LoadB:
                    {
                        AddEdge(instruction.C, instruction.Context.Pc, _currentBlock, BranchType.Always);
                        break;
                    }
                case OpCode.JumpBack:
                case OpCode.Jump:
                    {
                        AddEdge(instruction.D, instruction.Context.Pc, _currentBlock, BranchType.Always);
                        break;
                    }
                case OpCode.ForNPrep:
                    {
                        AddEdge(0, instruction.Context.Pc, _currentBlock, BranchType.Always);
                        break;
                    }
                case OpCode.Return:
                    {
                        AddEdge(0, instruction.Context.Pc, _currentBlock, BranchType.Never);
                        break;
                    }
                case OpCode.JumpXEqKNil:
                case OpCode.JumpXEqKB:
                case OpCode.JumpXEqKN:
                case OpCode.JumpXEqKS:
                    {
                        AddEdge(instruction.D == 1 ? 0 : instruction.D, instruction.Context.Pc, _currentBlock, BranchType.Can);
                        break;
                    }
                case OpCode.ForNLoop:
                case OpCode.JumpIf:
                case OpCode.JumpIfNot:
                case OpCode.JumpIfEq:
                case OpCode.JumpIfNotEq:
                case OpCode.JumpIfLe:
                case OpCode.JumpIfNotLe:
                case OpCode.JumpIfLt:
                case OpCode.JumpIfNotLt:
                    {
                        AddEdge(instruction.D, instruction.Context.Pc, _currentBlock, BranchType.Can);
                        break;
                    }
            }

            if (_currentBlock.Instructions.Count != 0)
                Blocks.Add(_currentBlock); // will fail if the block is already in the set
        }

        private void AddEdge(int jmp, int pc, BasicBlock block, BranchType type)
        {
            block.Branch = type;

            if (type != BranchType.Never)
            {
                _currentBlock = new BasicBlock();

                if (type == BranchType.Can)
                    block.AddEdge(_currentBlock, "false");

                var label = type == BranchType.Can ? "true" : null;

                // If the instruction comes after the current instruction, we need to utilize the edge queue.
                if (jmp > 0)
                    _edgeQueue.Add((jmp + pc, block, label));
                else if (jmp < 0)
                {
                    // Now we need to try and find the block that we are branching to. We need to check each block 
                    // to see if it contains the instruction we are branching to.
                    foreach (var targetBlock in Blocks)
                    {
                        if (targetBlock.Instructions.Any(instruction => instruction.Context.Pc == pc + jmp + 1))
                        {
                            block.AddEdge(targetBlock, label);
                            break;
                        }
                    }
                }
                else if (jmp == 0)
                {
                    // If the jump is zero, we are branching to the next instruction.
                    block.AddEdge(_currentBlock);
                }
            }
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
