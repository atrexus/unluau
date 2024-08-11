using Unluau.IR.ProtoTypes.Constants;
using Unluau.IR.ProtoTypes.ControlFlow;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ProtoTypes
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
        /// If the function is the main function.
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// The flags of the function prototype.
        /// </summary>
        public Flags? Flags { get; set; }

        /// <summary>
        /// The size of the instructions in the function prototype.
        /// </summary>
        public int InstructionSize { get; set; }

        /// <summary>
        /// The control flow graph of the function prototype.
        /// </summary>
        public Graph ControlFlow { get; set; } = new();

        /// <summary>
        /// The list of constants in the function prototype.
        /// </summary>
        public List<Constant> Constants { get; set; } = [];

        /// <summary>
        /// The list of prototypes in the function prototype.
        /// </summary>
        public List<ProtoType> ProtoTypes { get; set; } = [];

        /// <summary>
        /// The line that the function was defined on.
        /// </summary>
        public int LineDefined { get; set; }

        /// <summary>
        /// The last line that the function was defined on.
        /// </summary>
        public int? LastLineDefined { get; set; }

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

        /// <summary>
        /// Gets the name of a local variable by its register.
        /// </summary>
        /// <param name="register">The register to search for.</param>
        /// <returns>The name of the local.</returns>
        public string? GetLocalName(int register)
        {
            foreach (var local in Locals)
            {
                if (local.Register == register)
                    return local.Name;
            }

            return null;
        }

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
