using System.CodeDom.Compiler;

namespace Unluau.Decompile.IL.Writers
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
    }
}
