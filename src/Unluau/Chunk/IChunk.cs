using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IL;

namespace Unluau.Chunk
{
    /// <summary>
    /// Represents a chunk of compiled machine code.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Creates a chunk from an input stream.  
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <returns>A new chunk.</returns>
        public static abstract IChunk Create(Stream stream);

        /// <summary>
        /// Translates the current chunk to the IL.
        /// </summary>
        /// <returns>A block that wraps the IL instructions.</returns>
        public BasicBlock Translate();
    }
}
