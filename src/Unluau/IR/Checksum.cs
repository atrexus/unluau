using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IR
{
    /// <summary>
    /// The checksum of a stream.
    /// </summary>
    /// <param name="hashAlgorithm">The algorithm to use.</param>
    /// <param name="stream">The stream.</param>
    public class Checksum(HashAlgorithm hashAlgorithm, Stream stream, string source)
    {
        public readonly byte[] _hash = hashAlgorithm.ComputeHash(stream);

        /// <summary>
        /// The hash algorithm to use when computing the checksum.
        /// </summary>
        public HashAlgorithm HashAlgorithm { get; } = hashAlgorithm;

        /// <summary>
        /// The source file of the checksum.
        /// </summary>
        public string Source { get; } = source;

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var b in _hash)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }

        /// <summary>
        /// Verifies the hash against the computed hash.
        /// </summary>
        /// <param name="hash">The hash to verify.</param>
        public bool Verify(string hash)
        {
            var hashOfInput = ToString();

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
