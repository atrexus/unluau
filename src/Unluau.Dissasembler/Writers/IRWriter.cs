using System.Text;
using System.Xml.Linq;
using Unluau.Common.IR;
using Unluau.Common.IR.ProtoTypes;
using Unluau.Common.IR.Versions;

namespace Unluau.Disassembler.Writers
{
    /// <summary>
    /// Writes the IR to a stream.
    /// </summary>
    public class IRWriter : Visitor
    {
        private const string _tab = "   ";

        private readonly StreamWriter _stream;
        private Module? _module;
        private ProtoType? _protoType;
        private int _pc = 0;

        public IRWriter(Stream stream) => _stream = new(stream) { AutoFlush = true };

        /// <summary>
        /// Writes the main module to the stream.
        /// </summary>
        /// <param name="module">The module.</param>
        public override bool Visit(Module module)
        {
            var version = new StringBuilder(module.Version.Number.ToString());

            if (module.Version is TypedVersion typedVersion)
                version.Append($".{(byte)typedVersion.Kind}");

            _stream.WriteLine($"; Unluau disassembler version {MetaData.Version}, time: {module.ElapsedTime.TotalSeconds}s");
            _stream.WriteLine($"{module.Checksum.Source}: Luau bytecode executable, version {version}, hash: 0x{module.Checksum}\n");

            _module = module;
            return true;
        }

        /// <summary>
        /// Writes a function prototype to the stream.
        /// </summary>
        /// <param name="protoType">The prototype.</param>
        public override bool Visit(ProtoType protoType)
        {
            _protoType = protoType;

            // First we write the prototype header with basic info
            _stream.WriteLine($"{(protoType.IsMain ? "main" : "function")} <{_module!.Checksum.Source}:{protoType.LineDefined}> ({protoType.Instructions.Count} instructions, {protoType.InstructionSize} bytes)");

            // Now we write function specific information
            _stream.WriteLine($"{protoType.ParameterCount}{(protoType.IsVararg ? "+" : "")} params, " +
               $"{protoType.MaxStackSize} slots, {protoType.Upvalues.Count} upvalues, {protoType.Constants.Count} constants");

            _stream.Write($"function {(protoType.IsMain ? "main" : (protoType.Name ?? "prototype"))}(");

            for (int i = 0; i < protoType.ParameterCount; ++i)
            {
                _stream.Write($"v{i + 1}");

                if (i + 1 < protoType.ParameterCount + (protoType.IsVararg ? 1 : 0))
                    _stream.Write(", ");
            }

            if (protoType.IsVararg)
                _stream.Write("...");

            var lastLineDefined = protoType.Instructions.Last().LineDefined;

            if (lastLineDefined != null)
                _stream.WriteLine($") -- line {protoType.LineDefined} through {lastLineDefined}");

            // Now we write all of the instructions to the stream.
            foreach (var instruction in protoType.Instructions)
            {
                var pc = (_pc + 1).ToString().PadRight(5);
                _stream.Write($"{_tab}{pc}");

                // Now accept the instruction into the current visitor.
                instruction.Accept(this);

                _stream.WriteLine();
                ++_pc;
            }

            if (protoType.Constants.Count > 0)
            {
                // Now lets write all of the constants to the stream.
                _stream.WriteLine($"\n{_tab}constants ({protoType.Constants.Count})");
                _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", "index", "type", "value")}");

                for (int i = 0; i < protoType.Constants.Count; ++i)
                {
                    var constant = protoType.Constants[i];

                    _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", i, constant.Type.ToString().ToLower(), constant)}");
                }
            }

            if (protoType.Locals.Count > 0)
            {
                // Now lets write all of the locals to the stream.
                _stream.WriteLine($"\n{_tab}locals ({protoType.Locals.Count})");
                _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2, -8} {3, -8} {4}", "index", "name", "startpc", "endpc", "type")}");

                for (int i = 0; i < protoType.Locals.Count; ++i)
                {
                    var local = protoType.Locals[i];

                    _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2, -8} {3, -8} {4}", i, local.Name, local.Scope.Item1, local.Scope.Item2, local.Type)}");
                }
            }

            if (protoType.Upvalues.Count > 0)
            {
                // Now lets write all of the upvalues to the stream.
                _stream.WriteLine($"\n{_tab}upvalues ({protoType.Upvalues.Count})");
                _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", "index", "name", "type")}");

                for (int i = 0; i < protoType.Upvalues.Count; ++i)
                {
                    var upvalue = protoType.Upvalues[i];

                    _stream.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", i, upvalue.Name, upvalue.Type)}");
                }
            }

            _stream.WriteLine("end");

            return false;
        }

        /// <summary>
        /// Writes an instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(Instruction instruction)
        {
            var opCode = instruction.Code.ToString().ToUpper().PadRight(20);

            _stream.Write(opCode);
            return false;
        }

        /// <summary>
        /// Writes an ABC instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionABC instruction)
        {
            Visit(instruction as Instruction);

            _stream.Write($"{instruction.A} {instruction.B} {instruction.C}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes an AB instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionAB instruction)
        {
            Visit(instruction as Instruction);

            _stream.Write($"{instruction.A} {instruction.B}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes an AB instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionAD instruction)
        {
            Visit(instruction as Instruction);

            _stream.Write($"{instruction.A} {instruction.D}");
            WriteAux(instruction.Aux);
            return false;
        }

        /// <summary>
        /// Writes the auxiliary information for an instruction.
        private void WriteAux(Instruction? aux)
        {
            if (aux == null)
                return;

            _stream.WriteLine();

            _stream.Write("   " + (++_pc + 1).ToString().PadRight(5));
            _stream.Write("".PadRight(20));
            _stream.Write(aux.Value.ToString());
        }
    }
}
