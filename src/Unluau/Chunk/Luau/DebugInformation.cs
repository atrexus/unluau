using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.Utils;

namespace Unluau.Chunk.Luau
{
    public struct LocalVariable
    {
        /// <summary>
        /// The index to the symbol table (contains name).
        /// </summary>
        public int SymbolIndex { get; set; }

        /// <summary>
        /// The scope of the varible in the program.
        /// </summary>
        public (int, int) Scope { get; set; }

        /// <summary>
        /// Gets the register slot the variable lives in (relative to function).
        /// </summary>
        public byte Slot { get; set; }
    }

    /// <summary>
    /// Contains additional information about the function it belongs to.
    /// </summary>
    public class DebugInformation 
    {
        /// <summary>
        /// A list of local variables and their information.
        /// </summary>
        public LocalVariable[] LocalVariables { get; set; }

        /// <summary>
        /// A list of symbol indices for local upvalues.
        /// </summary>
        private int[] UpvalueSymbolIndeces { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="DebugInformation"/>.
        /// </summary>
        /// <param name="reader">The binary reader to use.</param>
        public DebugInformation(BinaryReader reader)
        {
            var localVariableCount = reader.ReadSize();

            LocalVariables = new LocalVariable[localVariableCount];

            for (int i = 0; i < localVariableCount; i++)
            {
                LocalVariables[i] = new()
                {
                    SymbolIndex = reader.ReadSize(),
                    Scope = (reader.ReadSize(), reader.ReadSize()),
                    Slot = reader.ReadByte(),
                };
            }
            
            var upvalueCount = reader.ReadSize();

            UpvalueSymbolIndeces = new int[upvalueCount];

            for (int i = 0;i < upvalueCount; i++)
                UpvalueSymbolIndeces[i] = reader.ReadSize();
        }
    }
}
