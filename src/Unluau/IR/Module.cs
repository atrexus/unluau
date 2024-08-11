using Unluau.IR.ProtoTypes;
using Version = Unluau.IR.Versions.Version;

namespace Unluau.IR
{
    /// <summary>
    /// A low level representation of a Luau module.
    /// </summary>
    public class Module : Node
    {
        /// <summary>
        /// The computed checksum of the module.
        /// </summary>
        public required Checksum Checksum { get; set; }

        /// <summary>
        /// The version of the module.
        /// </summary>
        public required Version Version { get; set; }

        /// <summary>
        /// The table of symbols in the module.
        /// </summary>
        public List<string> SymbolTable { get; set; } = [];

        /// <summary>
        /// The list of prototypes in the module.
        /// </summary>
        public List<ProtoType> ProtoTypes { get; set; } = [];

        /// <summary>
        /// The entry point of the module.
        /// </summary>
        public required int EntryPoint { get; set; }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var proto in ProtoTypes)
                    proto.Accept(visitor);
            }
        }
    }
}
