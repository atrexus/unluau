using Microsoft.Extensions.Logging;
using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ControlFlow
{
    /// <summary>
    /// Builds the control flow graph from an IR module.
    /// </summary>
    public class ControlFlowBuilder : Visitor
    {
        private readonly ILogger<ControlFlowBuilder> _logger;
        private readonly List<(int, BasicBlock, string?)> _edgeQueue = [];
        private readonly HashSet<BasicBlock> _blocks = [];
        private BasicBlock _entryBlock = new();
        private BasicBlock _currentBlock = new();

        /// <summary>
        /// Creates a new instance of the <see cref="ControlFlowBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to use.</param>
        private ControlFlowBuilder(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger<ControlFlowBuilder>();

        /// <summary>
        /// Builds the control flow graphs for all function prototypes in the module.
        /// </summary>
        public static void Build(ILoggerFactory loggerFactory, Module module)
        {
            var builder = new ControlFlowBuilder(loggerFactory);
            module.Accept(builder);
        }

        public override bool Visit(ProtoType protoType)
        {
            // reset all objects that we will use
            _edgeQueue.Clear();
            _blocks.Clear();
            _currentBlock = new BasicBlock();
            _entryBlock = _currentBlock;

            var name = protoType.IsMain ? "main" : (protoType.Name ?? $"prototype_{protoType.GetHashCode():x}");

            _logger.LogInformation("Building control flow graph for prototype '{}'", name);

            foreach (var instruction in protoType.Instructions)
                instruction.Accept(this);

            protoType.ControlFlow = [.. _blocks];

            return false;
        }

        public override bool Visit(Instruction instruction)
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
                    {
                        _currentBlock = new();
                        _logger.LogDebug("Creating new basic block {:x}", _currentBlock.Id);
                    }

                    block.AddEdge(_currentBlock.Id, label);
                    _edgeQueue.RemoveAt(i);
                    i--;

                    _logger.LogDebug("Added forwards edge \"{}\" {:x} => {:x}", label ?? "always branch", block.Id, _currentBlock.Id);
                }
            }

            _currentBlock.Instructions.Add(instruction);

            // If we have created a new block, and the old one has no branches, we need to add an edge to the new block.
            // This way this block will be reachable from the old block.
            if (_currentBlock != oldBlock && oldBlock.Branch is null)
            {
                oldBlock.AddEdge(_currentBlock.Id);
                _logger.LogDebug("Added forwards edge \"always branch\" {:x} => {:x}", oldBlock.Id, _currentBlock.Id);
            }

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
                _blocks.Add(_currentBlock); // will fail if the block is already in the set


            return false;
        }

        private void AddEdge(int jmp, int pc, BasicBlock block, BranchType type)
        {
            block.Branch = type;

            if (type != BranchType.Never)
            {
                _currentBlock = new BasicBlock();

                if (type == BranchType.Can)
                {
                    block.AddEdge(_currentBlock.Id, "false");
                    _logger.LogDebug("Added forwards edge \"false\" {:x} => {:x}", block.Id, _currentBlock.Id);
                }

                var label = type == BranchType.Can ? "true" : null;

                // If the instruction comes after the current instruction, we need to utilize the edge queue.
                if (jmp > 0)
                    _edgeQueue.Add((jmp + pc, block, label));
                else if (jmp < 0)
                {
                    // Now we need to try and find the block that we are branching to. We need to check each block 
                    // to see if it contains the instruction we are branching to.
                    foreach (var targetBlock in _blocks)
                    {
                        if (targetBlock.Instructions.Any(instruction => instruction.Context.Pc == pc + jmp + 1))
                        {
                            block.AddEdge(targetBlock.Id, label);
                            _logger.LogDebug("Added backwards edge \"{}\" {:x} <= {:x}", label ?? "always branch", targetBlock.Id, block.Id);
                            break;
                        }
                    }
                }
                else if (jmp == 0)
                {
                    // If the jump is zero, we are branching to the next instruction.
                    block.AddEdge(_currentBlock.Id);
                    _logger.LogDebug("Added forwards edge \"{}\" {:x} => {:x}", label ?? "always branch", block.Id, _currentBlock.Id);
                }
            }
        }
    }
}
