using Unluau.Common.IR.ProtoTypes;
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
    }
}
