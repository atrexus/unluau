namespace Unluau.Common.IR.ProtoTypes
{
    [Flags]
    public enum ProtoTypeFlags : byte
    {
        /// <summary>
        /// Used to tag main proto for modules with --!native
        /// </summary>
        NativeModule = 1 << 0,

        /// <summary>
        /// Used to tag individual protos as not profitable to compile natively
        /// </summary>
        NativeCold = 1 << 1,
    }

    /// <summary>
    /// Contains all sorts of flags for the function prototype.
    /// </summary>
    public class Flags(byte b) : Node
    {
        private readonly byte _flags = b;

        /// <summary>
        /// Returns whether the prototype has a specific flag.
        /// </summary>
        /// <param name="flag">The flag</param>
        public bool HasFlag(ProtoTypeFlags flag) => (_flags & (byte)flag) != 0;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
