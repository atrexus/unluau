using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR
{
    /// <summary>
    /// Decodes parts of the Luau bytecode during lifting.
    /// </summary>
    public class Decoder
    {
        /// <summary>
        /// Decodes the opcode of the instruction.
        /// </summary>
        public virtual OpCode DecodeOpCode(byte data) => (OpCode)data;
    }
}
