namespace Unluau.Common.IR.Versions
{
    /// <summary>
    /// Represents a version of Luau bytecode.
    /// </summary>
    public class Version : Node
    {
        public static readonly byte MinVersion = 3;
        public static readonly byte MaxVersion = 5;

        /// <summary>
        /// The version number.
        /// </summary>
        public byte Number { get; set; }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

        public static bool IsSupported(byte version) => version <= MaxVersion && version >= MinVersion;
    }
}
