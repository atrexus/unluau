using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Decompile.Utils
{
    public static class BinaryReaderExtentions
    {
        /// <summary>
        /// Reads a compressed 32-bit integer from the binary stream (Luau bytecode is sometimes compressed).
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <returns>An uncompressed integer.</returns>
        public static int ReadSize(this BinaryReader reader)
        {
            int result = 0;
            int shift = 0;

            byte b;

            // I'm going to be honest, I have no idea why Luau does this. It seems to be because oftentimes size values are too large, 
            // and can be a hastle to read. What is weird is that no other Lua version does something like this...
            do
            {
                b = reader.ReadByte();
                result |= (b & 127) << shift;
                shift += 7;
            } while ((b & 128) > 0);

            return result;
        }

        /// <summary>
        /// Reads an optional index value from the bytecode stream (for Luau).
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <returns>The integer.</returns>
        public static int? ReadIndex(this BinaryReader reader)
        {
            var value = reader.ReadSize();

            if (value == 0)
                return null;

            return value;
        }

        /// <summary>
        /// Reads an ASCII string with the specified length in bytes.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="length">The length of the string.</param>
        /// <returns>A new ASCII string.</returns>
        public static string ReadASCII(this BinaryReader reader, int length)
        {
            byte[] bytes = reader.ReadBytes(length);

            if (bytes.Length != length)
                throw new EndOfStreamException("Remaining size of stream was smaller than requested number of bytes.");

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
