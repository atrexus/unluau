using Unluau.Common.IR;
using Unluau.Disassembler.Writers;

namespace Unluau.Disassembler
{
    /// <summary>
    /// Writes an IR module to a stream.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Writes the IR module to the provided stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="module">The module.</param>
        /// <param name="writerType">The type of writer.</param>
        public static void WriteTo(Stream stream, Module module, WriterType writerType = WriterType.IR)
        {
            Visitor writer = writerType switch
            {
                WriterType.IR => new IRWriter(stream),
                _ => throw new ArgumentOutOfRangeException(nameof(writerType))
            };

            module.Accept(writer);
        }
    }
}
