using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.Decoders
{
    /// <summary>
    /// Decodes parts of the Luau bytecode during lifting.
    /// </summary>
    public class Decoder
    {
        /// <summary>
        /// Decodes the opcode of the instruction.
        /// </summary>
        public virtual byte DecodeOpCode(byte data) => data;

        /// <summary>
        /// Decodes a 32-bit raw instruction.
        /// </summary>
        public virtual uint DecodeInstruction(uint data) => data;

        /// <summary>
        /// Decodes a 64-bit raw instruction.
        /// </summary>
        public virtual ulong DecodeInstruction(ulong data) => data;
    }
}
