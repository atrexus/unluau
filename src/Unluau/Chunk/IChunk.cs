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
        /// Lifts the current chunk to an IL program.
        /// </summary>
        /// <returns>An IL program.</returns>
        public abstract Program Lift();
    }
}
