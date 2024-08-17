using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.Decoders
{
    /// <summary>
    /// Implements the decoder for Roblox bytecode. Roblox bytecode encodes the opcodes of the instructions.
    /// </summary>
    public class RobloxDecoder : Decoder
    {
        /// <summary>
        /// Decodes a raw opcode of an instruction.
        /// </summary>
        public override byte DecodeOpCode(byte data) => (byte)(data * 203);

        /// <summary>
        /// Decodes a 32-bit raw instruction.
        /// </summary>
        public override uint DecodeInstruction(uint data) => (data & ~0xFFU) | DecodeOpCode((byte)data);

        /// <summary>
        /// Decodes a 64-bit raw instruction.
        /// </summary>
        public override ulong DecodeInstruction(ulong data) => (data & ~0xFFUL) | DecodeOpCode((byte)data);
    }
}
