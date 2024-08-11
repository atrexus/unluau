using Unluau.IR;

namespace Unluau.IR.Writers
{
    /// <summary>
    /// The base class for all writers.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public abstract class Writer(Stream stream) : Visitor
    {
        /// <summary>
        /// The writer used to write to the stream.
        /// </summary>
        protected readonly StreamWriter _writer = new(stream) { AutoFlush = true };

        /// <summary>
        /// Writes the result of the lifting to the stream.
        /// </summary>
        /// <param name="result">The lifting result.</param>
        public abstract void Write(LiftResult result);
    }
}
