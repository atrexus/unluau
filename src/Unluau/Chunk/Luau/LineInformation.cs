using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Decompile.Chunk.Luau
{
    /// <summary>
    /// Describes the location of different instructions within the original program.
    /// </summary>
    public class LineInformation
    {
        private readonly byte _lineGapLog2;
        private readonly byte[] _lineInfo;
        private readonly int[] _absLineInfo;

        /// <summary>
        /// Creates a new intance of <see cref="LineInformation"/>.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="instructionCount">The number of instructions in the current function.</param>
        public LineInformation(BinaryReader reader, int instructionCount) 
        { 
            _lineGapLog2 = reader.ReadByte();

            int intervals = ((instructionCount - 1) >> _lineGapLog2) + 1;
            int absOffset = (instructionCount + 3) & ~3;

            int lineInfoCount = absOffset + intervals * sizeof(int);
 
            _lineInfo = new byte[lineInfoCount];
            _absLineInfo = new int[lineInfoCount - absOffset];

            byte lastOffset = 0;
            int lastLine = 0;

            for (int i = 0; i < instructionCount; i++)
                _lineInfo[i] = lastOffset += reader.ReadByte();

            for (int i = 0; i < intervals; i++)
                _absLineInfo[i] = lastLine += reader.ReadInt32();
        }

        /// <summary>
        /// Gets the line number for an instruction.
        /// </summary>
        /// <param name="pc">The instruction's index in the function (program counter).</param>
        /// <returns>The line.</returns>
        public int GetLine(int pc) => _absLineInfo[pc >> _lineGapLog2] + _lineInfo[pc];

        /// <summary>
        /// Gets the line numbers for a range of instructions.
        /// </summary>
        /// <param name="startPc">Start instruction index.</param>
        /// <param name="endPc">End instruction index.</param>
        /// <returns>The rane of lines.</returns>
        public (int, int) GetLines(int startPc, int endPc) => (GetLine(startPc), GetLine(endPc));
    }
}
