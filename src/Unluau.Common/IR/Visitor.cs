using Unluau.Common.IR.ProtoTypes;
using Unluau.Common.IR.ProtoTypes.Instructions;
using Unluau.Common.IR.Versions;
using Type = Unluau.Common.IR.ProtoTypes.Type;
using Version = Unluau.Common.IR.Versions.Version;

namespace Unluau.Common.IR
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
        /// The visitor for the <see cref="Instruction"/> class.
        /// </summary>
        public virtual bool Visit(Instruction instruction) => Visit(instruction as Node);

        /// <summary>
        /// The visitor for the <see cref="InstructionABC"/> class.
        /// </summary>
        public virtual bool Visit(InstructionABC instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionAD"/> class.
        /// </summary>
        public virtual bool Visit(InstructionAD instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="InstructionE"/> class.
        /// </summary>
        public virtual bool Visit(InstructionE instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="Nop"/> class.
        /// </summary>
        public virtual bool Visit(Nop instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="Break"/> class.
        /// </summary>
        public virtual bool Visit(Break instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// The visitor for the <see cref="LoadNil"/> class.
        /// </summary>
        public virtual bool Visit(LoadNil instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="LoadBoolean"/> class.
        /// </summary>
        public virtual bool Visit(LoadBoolean instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="LoadNumber"/> class.
        /// </summary>
        public virtual bool Visit(LoadNumber instruction) => Visit(instruction as InstructionAD);

        /// <summary>
        /// The visitor for the <see cref="LoadK"/> class.
        /// </summary>
        public virtual bool Visit(LoadK instruction) => Visit(instruction as InstructionAD);

        /// <summary>
        /// The visitor for the <see cref="Move"/> class.
        /// </summary>
        public virtual bool Visit(Move instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="GetGlobal"/> class.
        /// </summary>
        public virtual bool Visit(GetGlobal instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="SetGlobal"/> class.
        /// </summary>
        public virtual bool Visit(SetGlobal instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="GetUpvalue"/> class.
        /// </summary>
        public virtual bool Visit(GetUpvalue instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="SetUpvalue"/> class.
        /// </summary>
        public virtual bool Visit(SetUpvalue instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="CloseUpvalues"/> class.
        /// </summary>
        public virtual bool Visit(CloseUpvalues instruction) => Visit(instruction as InstructionABC);

        /// <summary>
        /// The visitor for the <see cref="GetImport"/> class.
        /// </summary>
        public virtual bool Visit(GetImport instruction) => Visit(instruction as InstructionAD);
    }
}
