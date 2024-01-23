using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.Utils;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// Describes the version of the type encoding.
    /// </summary>
    public enum TypesVersionKind : byte
    {
        /// <summary>
        /// First version (experimental).
        /// </summary>
        Version1 = 1
    }

    /// <summary>
    /// Represents a version of Luau bytecode.
    /// </summary>
    public class Version
    {
        private readonly (byte, byte) _supportedVersions = (3, 5);

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public byte VersionNumber { get; set; }

        /// <summary>
        /// Gets the version number for the type encoding.
        /// </summary>
        public TypesVersionKind? TypesVersion { get; set; }

        /// <summary>
        /// Is true if the current bytecode version requires type encoding.
        /// </summary>
        public bool HasTypeEncoding => TypesVersion != null;

        /// <summary>
        /// Creates a new instance of <see cref="Version"/>.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        public Version(BinaryReader reader)
        {
            VersionNumber = reader.ReadByte();

            // If the version is absent, we know that the rest of the bytecode will contain an error message.
            if (VersionNumber == 0)
                throw new Exception(reader.ReadASCII((int)(reader.BaseStream.Length - 1)));

            // Make sure we have a valid bytecode version (so in range)
            if (VersionNumber < _supportedVersions.Item1 || VersionNumber > _supportedVersions.Item2)
                throw new Exception($"Bytecode version mismatch, expected version {_supportedVersions.Item1}...{_supportedVersions.Item2}");

            if (VersionNumber >= 4)
            {
                var versionNumber = reader.ReadByte();

                if (!Enum.IsDefined(typeof(TypesVersionKind), versionNumber))
                    throw new Exception($"Types version mismatch, got version {versionNumber}");

                TypesVersion = (TypesVersionKind)versionNumber;
            }
        }
    }
}
