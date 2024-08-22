using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Instructions;
using Unluau.IR.ProtoTypes.Constants;
using Unluau.IR.Versions;
using Type = Unluau.IR.ProtoTypes.Type;
using Version = Unluau.IR.Versions.Version;
using Constant = Unluau.IR.ProtoTypes.Constants.Constant;
using Unluau.IR.ControlFlow.Nodes;

namespace Unluau.IR
{
    /// <summary>
    /// The base visitor class for the IR instructions. Methods return true if the node's children are 
    /// to be visited as well.
    /// </summary>
    public abstract class Visitor
    {
        /// <summary>
        /// The base visitor for the <see cref="Node"/> class.
        /// </summary>
        public virtual bool Visit(Node node) => true;

        /// <summary>
        /// The visitor for the <see cref="Module"/> class.
        /// </summary>
        public virtual bool Visit(Module module) => Visit(module as Node);

        /// <summary>
        /// The visitor for the <see cref="Version"/> class.
        /// </summary>
        public virtual bool Visit(Version version) => Visit(version as Node);

        /// <summary>
        /// The visitor for the <see cref="Version"/> class.
        /// </summary>
        public virtual bool Visit(TypedVersion version) => Visit(version as Version);

        /// <summary>
        /// The visitor for the <see cref="ProtoType"/> class.
        /// </summary>
        public virtual bool Visit(ProtoType proto) => Visit(proto as Node);

        /// <summary>
        /// The visitor for the <see cref="Flags"/> class.
        /// </summary>
        public virtual bool Visit(Flags flags) => Visit(flags as Node);

        /// <summary>
        /// The visitor for the <see cref="Local"/> class.
        /// </summary>
        public virtual bool Visit(Local local) => Visit(local as Node);

        /// <summary>
        /// The visitor for the <see cref="Upvalue"/> class.
        /// </summary>
        public virtual bool Visit(Upvalue upvalue) => Visit(upvalue as Node);

        /// <summary>
        /// The visitor for the <see cref="Type"/> class.
        /// </summary>
        public virtual bool Visit(Type type) => Visit(type as Node);

        /// <summary>
        /// The visitor for the <see cref="Constant"/> class.
        /// </summary>
        public virtual bool Visit(Constant constant) => Visit(constant as Node);

        /// <summary>
        /// The visitor for the <see cref="BooleanConstant"/> class.
        /// </summary>
        public virtual bool Visit(BooleanConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="ClosureConstant"/> class.
        /// </summary>
        public virtual bool Visit(ClosureConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="ImportConstant"/> class.
        /// </summary>
        public virtual bool Visit(ImportConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="NilConstant"/> class.
        /// </summary>
        public virtual bool Visit(NilConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="NumberConstant"/> class.
        /// </summary>
        public virtual bool Visit(NumberConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="StringConstant"/> class.
        /// </summary>
        public virtual bool Visit(StringConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="TableConstant"/> class.
        /// </summary>
        public virtual bool Visit(TableConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="VectorConstant"/> class.
        /// </summary>
        public virtual bool Visit(VectorConstant constant) => Visit(constant as Constant);

        /// <summary>
        /// The visitor for the <see cref="Instruction"/> class.
        /// </summary>
        public virtual bool Visit(Instruction instruction) => Visit(instruction as Node);

        /// <summary>
        /// The visitor for the <see cref="InstructionABC"/> class.
        /// </summary>
        public virtual bool Visit(InstructionABC instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionAB"/> class.
        /// </summary>
        public virtual bool Visit(InstructionAB instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionA"/> class.
        /// </summary>
        public virtual bool Visit(InstructionA instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionAD"/> class.
        /// </summary>
        public virtual bool Visit(InstructionAD instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionE"/> class.
        /// </summary>
        public virtual bool Visit(InstructionE instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="Edge"/> class.
        /// </summary>
        public virtual bool Visit(Edge edge) => Visit(edge as Node);

        /// <summary>
        /// The visitor for the <see cref="BasicBlock"/> class.
        /// </summary>
        public virtual bool Visit(BasicBlock block) => Visit(block as Node);

        /// <summary>
        /// The visitor for the <see cref="CodeBlock"/> class.
        /// </summary>
        public virtual bool Visit(CodeBlock block) => Visit(block as BasicBlock);

        /// <summary>
        /// The visitor for the <see cref="AbstractBlock"/> class.
        /// </summary>
        public virtual bool Visit(AbstractBlock block) => Visit(block as BasicBlock);

        /// <summary>
        /// The visitor for the <see cref="SequentialBlock"/> class.
        /// </summary>
        public virtual bool Visit(SequentialBlock block) => Visit(block as AbstractBlock);

        /// <summary>
        /// The visitor for the <see cref="IfThenBlock"/> class.
        /// </summary>
        public virtual bool Visit(IfThenBlock block) => Visit(block as AbstractBlock);
    }
}
