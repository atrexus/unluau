namespace Unluau.Common.IR.Versions
{
    /// <summary>
    /// Represents a version of Luau bytecode.
    /// </summary>
    public class Version(byte number) : Node
    {
        public static readonly byte MinVersion = 3;
        public static readonly byte MaxVersion = 5;

        /// <summary>
        /// The version number.
        /// </summary>
        public byte Number { get; set; } = number;

        /// <summary>
        /// Creates a new instance of <see cref="Version"/>.
        /// </summary>
        public Version() : this(0)
        {
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Returns whether the version is supported.
        /// </summary>
        /// <param name="version">The version number.</param>
        public static bool IsSupported(byte version) => version <= MaxVersion && version >= MinVersion;

        /// <summary>
        /// Returns whether the version is typed.
        /// </summary>
        /// <param name="version">The version number.</param>
        public static bool IsTyped(byte version) => version >= 4;
    }
}
