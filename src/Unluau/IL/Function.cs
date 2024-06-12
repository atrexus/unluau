using Unluau.Common.IR.ProtoTypes;
using Unluau.Decompile.IL.Blocks;
using Unluau.Decompile.IL.Instructions;
using Unluau.Decompile.IL.Values;

namespace Unluau.Decompile.IL
{
    /// <summary>
    /// Represents a function in the IL program.
    /// </summary>
    public class Function : Node
    {
        /// <summary>
        /// A list of variables that act as parameters to the closure.
        /// </summary>
        public Variable[] Parameters { get; set; }

        /// <summary>
        /// Whether the function is vararg.
        /// </summary>
        public bool IsVararg { get; set; }

        /// <summary>
        /// The name of the function.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The control flow of the function.
        /// </summary>
        public BasicBlock ControlFlow { get; set; }

        /// <summary>
        /// The stack of the function.
        /// </summary>
        public Stack Stack { get; set; }

        /// <summary>
        /// Whether the function is the entry point.
        /// </summary>
        public bool IsEntryPoint { get; set; }

        /// <summary>
        /// Creates a new <see cref="Function"/> instance.
        /// </summary>
        /// <param name="protoType">The function prototype.</param>
        public Function(ProtoType protoType) : base(new Context(protoType.Instructions))
        {
            Parameters = new Variable[protoType.ParameterCount];

            for (int slot = 0; slot < protoType.ParameterCount; ++slot)
                Parameters[slot] = new Variable(Context, slot, protoType.GetLocalName(slot));

            ControlFlow = [];

            IsVararg = protoType.IsVararg;
            Name = protoType.Name;

            Stack = new();
        }

        /// <summary>
        /// Visits the children of the closure.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void VisitChildren(Visitor visitor)
        {
            ControlFlow.Visit(visitor);

            foreach (var variable in Parameters)
                variable.Visit(visitor);
        }

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
                VisitChildren(visitor);
        }

        /// <summary>
        /// Adds a block to the function's control flow.
        /// </summary>
        /// <param name="block">The block.</param>
        public void AddBlock(BasicBlock block) => ControlFlow.Next = block;

        /// <summary>
        /// Adds an instruction to the control flow of the function.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public void AddInstruction(Instruction instruction) => ControlFlow.Add(instruction);
    }
}
