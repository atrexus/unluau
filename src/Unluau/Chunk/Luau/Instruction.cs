using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// Represents an instruction in a Luau bytecode chunk.
    /// </summary>
    /// <param name="value">The raw instruction value.</param>
    public class Instruction(int value)
    {
        /// <summary>
        /// Contains the raw instruction value.
        /// </summary>
        public int Value { get; private set; } = value;

        /// <summary>
        /// Gets the operation code for the instruction.
        /// </summary>
        public OpCode Code => (OpCode)(Value & 0xFF);

        /// <summary>
        /// Gets the A operand of the instruction.
        /// </summary>
        public byte A => (byte)((Value >> 8) & 0xFF);

        /// <summary>
        /// Gets the B operand of the instruction.
        /// </summary>
        public byte B => (byte)((Value >> 16) & 0xFF);

        /// <summary>
        /// Gets the C operand of the instruction.
        /// </summary>
        public byte C => (byte)((Value >> 24) & 0xFF);

        /// <summary>
        /// Gets the D operand of the instruction.
        /// </summary>
        public int D => Value >> 16;

        /// <summary>
        /// Gets the E operand of the instruction.
        /// </summary>
        public int E => Value >> 8;
    }
}
