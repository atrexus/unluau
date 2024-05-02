using System.Text;

using Version = Unluau.Common.IR.Versions.Version;

namespace Unluau.Common.IR
{
    /// <summary>
    /// This class builds the IR from a Luau bytecode stream.
    /// </summary>
    public class IRReader : BinaryReader
    {
        /// <summary>
        /// Creates a new instance of <see cref="IRReader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public IRReader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="IRReader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The encoding to use.</param>
        public IRReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="IRReader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Leaves the stream open.</param>
        public IRReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        /// <summary>
        /// Reads a <see cref="Module"/> from the stream.
        /// </summary>
        /// <returns>The module.</returns>
        public Module ReadModule()
        {
        }

        /// <summary>
        /// Reads a <see cref="Version"/> from the stream.
        /// </summary>
        public Version ReadVersion()
        {
            var num = ReadByte();

            // If the version number is 0, then the rest of the stream is a string.
            // The string contains an error message that the script generated.
            if (num == 0)
                throw new Exception(ReadString((int)(BaseStream.Length - 1)));

            if (!Version.IsSupported(num))
                throw new Exception($"Bytecode version mismatch, expected version {Version.MinVersion}...{Version.MaxVersion}");
        }

        /// <summary>
        /// Reads a string with a length of <paramref name="length"/> from the stream.
        /// </summary>
        public string ReadString(int? length = null)
        {
            length ??= Read7BitEncodedInt();

            return Encoding.ASCII.GetString(ReadBytes(length.Value));
        }
    }
}
