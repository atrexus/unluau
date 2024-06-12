using Unluau.Decompile.IL.Blocks;
using Unluau.Decompile.IL.Conditions;
using Unluau.Decompile.IL.Instructions;
using Unluau.Decompile.IL.Values;

namespace Unluau.Decompile.IL
{
    /// <summary>
    /// Represents a visitor for IL instructions.
    /// </summary>
    public class Visitor
    {
        /// <summary>
        /// Implements the visitor pattern for a <see cref="Node"/>.
        /// </summary>
        public virtual bool Visit(Node node) => true;

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Program"/>.
        /// </summary>
        public virtual bool Visit(Program program) => Visit(program as Node);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Function"/>.
        /// </summary>
        public virtual bool Visit(Function function) => Visit(function as Node);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="BasicBlock"/>.
        /// </summary>
        public virtual bool Visit(BasicBlock block) => Visit(block as Node);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Instruction"/>.
        /// </summary>
        public virtual bool Visit(Instruction instruction) => Visit(instruction as Node);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="LoadValue"/>.
        /// </summary>
        public virtual bool Visit(LoadValue instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="LoadValue"/>.
        /// </summary>
        public virtual bool Visit(Call instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Return"/>.
        /// </summary>
        public virtual bool Visit(Return instruction) => Visit(instruction as Instruction);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="BasicValue"/>.
        /// </summary>
        public virtual bool Visit(BasicValue basicValue) => Visit(basicValue as Node);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Variable"/>.
        /// </summary>
        public virtual bool Visit(Variable variable) => Visit(variable as BasicValue);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Global"/>.
        /// </summary>
        public virtual bool Visit(Global global) => Visit(global as BasicValue);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="CallResult"/>.
        /// </summary>
        public virtual bool Visit(CallResult callResult) => Visit(callResult as BasicValue);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="BasicCondition"/>.
        /// </summary>
        public virtual bool Visit(BasicCondition condition) => Visit(condition as BasicValue);

        /// <summary>
        /// Implements the visitor pattern for a <see cref="Equals"/>.
        /// </summary>
        public virtual bool Visit(Equals condition) => Visit(condition as BasicCondition);
    }
}
