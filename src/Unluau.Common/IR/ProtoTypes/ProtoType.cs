namespace Unluau.Common.IR.ProtoTypes
{
    /// <summary>
    /// A low level representation of a Lua function (its a "prototype" of a function).
    /// </summary
    public class ProtoType : Node
    {
        /// <summary>
        /// The total number of register slots the function requires.
        /// </summary>
        public byte MaxStackSize { get; set; }

        /// <summary>
        /// The number of parameters the function requires.
        /// </summary>
        public byte ParameterCount { get; set; }

        /// <summary>
        /// If the function is a vararg (...) function.
        /// </summary>
        public bool IsVararg { get; set; }

        /// <summary>
        /// The flags of the function prototype.
        /// </summary>
        public Flags? Flags { get; set; }

        /// <summary>
        /// The list of instructions in the function prototype.
        /// </summary>
        public List<Instruction> Instructions { get; set; } = [];

        /// <summary>
        /// The line that the function was defined on.
        /// </summary>
        public int LineDefined { get; set; }

        /// <summary>
        /// The name of the function prototype.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The list of local variables in the function prototype.
        /// </summary>
        public List<Local> Locals { get; set; } = [];

        /// <summary>
        /// The list of upvalues in the function prototype.
        /// </summary>
        public List<Upvalue> Upvalues { get; set; } = [];

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Flags?.Accept(visitor);

                foreach (var local in Locals)
                    local.Accept(visitor);

                foreach (var upvalue in Upvalues)
                    upvalue.Accept(visitor);
            }
        }
    }
}
