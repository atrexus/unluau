using System.Reflection;
using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.Writers
{
    /// <summary>
    /// Writes the IR to a stream.
    /// </summary>
    public class IRWriter(Stream stream) : Writer(stream)
    {
        private const string _tab = "   ";

        private Module? _module;
        private int _pc = 0;

        /// <summary>
        /// Writes the main module to the stream.
        /// </summary>
        /// <param name="module">The module.</param>
        public override bool Visit(Module module)
        {
            _writer.WriteLine($"{module.Checksum.Source}: Luau bytecode executable, version {module.Version}, hash: 0x{module.Checksum}\n");

            _module = module;
            return true;
        }

        /// <summary>
        /// Writes a function prototype to the stream.
        /// </summary>
        /// <param name="protoType">The prototype.</param>
        public override bool Visit(ProtoType protoType)
        {
            _pc = 0;

            // First we write the prototype header with basic info
            _writer.WriteLine($"{(protoType.IsMain ? "main" : "function")} <{_module!.Checksum.Source}:{protoType.LineDefined}> ({protoType.Instructions.Count} instructions, {protoType.InstructionSize} bytes)");

            // Now we write function specific information
            _writer.WriteLine($"{protoType.ParameterCount}{(protoType.IsVararg ? "+" : "")} params, " +
               $"{protoType.MaxStackSize} slots, {protoType.Upvalues.Count} upvalues, {protoType.Constants.Count} constants");

            _writer.Write($"function {(protoType.IsMain ? "main" : (protoType.Name ?? "prototype"))}(");

            for (int i = 0; i < protoType.ParameterCount; ++i)
            {
                _writer.Write($"v{i + 1}");

                if (i + 1 < protoType.ParameterCount + (protoType.IsVararg ? 1 : 0))
                    _writer.Write(", ");
            }

            if (protoType.IsVararg)
                _writer.Write("...");

            var lastLineDefined = protoType.LastLineDefined;

            if (lastLineDefined != null)
                _writer.WriteLine($") -- line {protoType.LineDefined} through {lastLineDefined}");

            // Now we write all of the instructions to the stream.
            foreach (var instruction in protoType.Instructions)
            {
                var pc = (_pc + 1).ToString().PadRight(5);
                _writer.Write($"{_tab}{pc}");

                // Now accept the instruction into the current visitor.
                instruction.Accept(this);

                _writer.WriteLine();
                ++_pc;
            }

            if (protoType.Constants.Count > 0)
            {
                // Now lets write all of the constants to the stream.
                _writer.WriteLine($"\n{_tab}constants ({protoType.Constants.Count})");
                _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", "index", "type", "value")}");

                for (int i = 0; i < protoType.Constants.Count; ++i)
                {
                    var constant = protoType.Constants[i];

                    _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", i, constant.Type.ToString().ToLower(), constant)}");
                }
            }

            if (protoType.Locals.Count > 0)
            {
                // Now lets write all of the locals to the stream.
                _writer.WriteLine($"\n{_tab}locals ({protoType.Locals.Count})");
                _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2, -8} {3, -8} {4}", "index", "name", "startpc", "endpc", "type")}");

                for (int i = 0; i < protoType.Locals.Count; ++i)
                {
                    var local = protoType.Locals[i];

                    _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2, -8} {3, -8} {4}", i, local.Name, local.Scope.Item1, local.Scope.Item2, local.Type)}");
                }
            }

            if (protoType.Upvalues.Count > 0)
            {
                // Now lets write all of the upvalues to the stream.
                _writer.WriteLine($"\n{_tab}upvalues ({protoType.Upvalues.Count})");
                _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", "index", "name", "type")}");

                for (int i = 0; i < protoType.Upvalues.Count; ++i)
                {
                    var upvalue = protoType.Upvalues[i];

                    _writer.WriteLine($"{_tab}{_tab}{string.Format("{0, -6} {1,-8} {2}", i, upvalue.Name, upvalue.Type)}");
                }
            }

            _writer.WriteLine("end");

            return false;
        }

        /// <summary>
        /// Writes an instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(Instruction instruction)
        {
            var opCode = instruction.Code.ToString().ToUpper().PadRight(20);

            _writer.Write(opCode);
            return false;
        }

        /// <summary>
        /// Writes an ABC instruction to the stream.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public override bool Visit(InstructionABC instruction)
        {
            Visit(instruction as Instruction);

            _writer.Write($"{instruction.A} {instruction.B} {instruction.C}");
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

            _writer.Write($"{instruction.A} {instruction.B}");
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

            _writer.Write($"{instruction.A} {instruction.D}");
            WriteAux(instruction.Aux);
            return false;
        }

        public override void Write(LiftResult result)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            _writer.WriteLine($"; unluau disassembler version {version}, elapsed: {result.ElapsedTime.TotalSeconds}s");

            result.Module.Accept(this);
        }

        /// <summary>
        /// Writes the auxiliary information for an instruction.
        private void WriteAux(Instruction? aux)
        {
            if (aux == null)
                return;

            _writer.WriteLine();

            _writer.Write("   " + (++_pc + 1).ToString().PadRight(5));
            _writer.Write("".PadRight(20));
            _writer.Write(aux.Value.ToString());
        }
    }
}
