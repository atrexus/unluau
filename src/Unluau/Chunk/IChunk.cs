using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
