using Unluau.Common.IR;
using Unluau.Common.IR.ProtoTypes;
using Unluau.Decompile.IL.Statements;
using Unluau.Decompile.IL.Statements.Blocks;

namespace Unluau.Decompile.Builders
{
    /// <summary>
    /// Builds the IL from the IR.
    /// </summary>
    public class ILBuilder : Visitor
    {
        /// <summary>
        /// Builds an IL program from the module.
        /// </summary>
        /// <param name="module">The module.</param>
        public static IL.Program Build(Module module)
        {
            var builder = new ILBuilder();
            module.Accept(builder);

            return new IL.Program(builder._mainClosure!.Context, builder._mainClosure, [.. builder._closures]);
        }

        private List<Closure> _closures;
        private Closure? _mainClosure;
        private IL.Stack? _stack;

        private List<BasicBlock> _blocks;

        private ILBuilder() 
        {
            _closures = [];
            _blocks = [];
        }

        /// <summary>
        /// Visits a <see cref="Module"/>.
        /// </summary>
        public override bool Visit(Module module)
        {
            // We will visit the main function prototype only. The main proto has pointers to all other functions in it.
            // This way we can build the entire program from the main function.
            module.ProtoTypes[module.EntryPoint].Accept(this);

            return false;
        }

        /// <summary>
        /// Visits a <see cref="ProtoType"/>.
        /// </summary>
        public override bool Visit(ProtoType protoType)
        {
            _stack = new();

            var context = new ClosureContext(protoType);

            // Now we load the parameters to this closure onto the stack.
            foreach (var variable in context.Parameters)
                _stack.Set(variable.Slot, variable);

            _blocks.Add(new BasicBlock());

            // Now we visit the instructions in the proto.
            foreach (var instruction in protoType.Instructions)
                instruction.Accept(this);

/*            var closure = new Closure(context, );

            // Update the closure appropriately.
            if (protoType.IsMain)
                _mainClosure = closure;
            else
                _closures.Add(closure);*/

            return false;
        }

        /// <summary>
        /// Visits an <see cref="Instruction"/>.
        /// </summary>
        public override bool Visit(InstructionAD instruction)
        {
            switch (instruction.Code)
            {
                case OpCode.GetImport:
                {

                    break;
                }
            }

            return false;
        }
    }
}
