using System.Reflection;
using System.Text.Json;
using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Constants;
using Unluau.IR.ProtoTypes.Instructions;
using Unluau.IR.Versions;

namespace Unluau.IR.Writers
{
    /// <summary>
    /// Writes IR objects to a stream in json format.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public class JsonWriter(Stream stream) : Writer(stream)
    {
        private readonly Utf8JsonWriter _jsonWriter = new(stream, new JsonWriterOptions() { Indented = true });
        private readonly HashSet<BasicBlock> _blocks = [];

        /// <summary>
        /// Writes the result to the stream.
        /// </summary>
        public override void Write(LiftResult result)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            _jsonWriter.WriteStartObject();
            _jsonWriter.WriteString("version", version!.ToString());
            _jsonWriter.WriteString("elapsed", result.ElapsedTime.ToString());

            _jsonWriter.WriteStartObject("module");
            result.Module.Accept(this);
            _jsonWriter.WriteEndObject();

            _jsonWriter.WriteEndObject();
            _jsonWriter.Flush();
        }

        public override bool Visit(Module module)
        {
            _jsonWriter.WriteString("hash", module.Checksum.ToString());

            _jsonWriter.WriteStartObject("version");
            module.Version.Accept(this);
            _jsonWriter.WriteEndObject();

            _jsonWriter.WriteStartArray("symbolTable");
            module.SymbolTable.ForEach(_jsonWriter.WriteStringValue);
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteStartObject("entryPoint");
            module.ProtoTypes[module.EntryPoint].Accept(this);
            _jsonWriter.WriteEndObject();

            return false;
        }

        public override bool Visit(TypedVersion version)
        {
            _jsonWriter.WriteNumber("major", version.Number);
            _jsonWriter.WriteNumber("typed", (int)version.Kind);
            return false;
        }

        public override bool Visit(Versions.Version version)
        {
            _jsonWriter.WriteNumber("major", version.Number);
            return false;
        }

        public override bool Visit(ProtoType proto)
        {
            _jsonWriter.WriteNumber("maxStackSize", proto.MaxStackSize);
            _jsonWriter.WriteNumber("parameterCount", proto.ParameterCount);
            _jsonWriter.WriteBoolean("isVararg", proto.IsVararg);

            if (proto.ControlFlow.Count == 0)
            {
                _jsonWriter.WriteStartArray("instructions");
                proto.Instructions.ForEach(instruction =>
                {
                    _jsonWriter.WriteStartObject();
                    instruction.Accept(this);
                    _jsonWriter.WriteEndObject();
                });
                _jsonWriter.WriteEndArray();
            }
            else
            {
                _jsonWriter.WriteStartArray("controlFlow");
                proto.ControlFlow.ForEach(block => 
                {
                    _jsonWriter.WriteStartObject();
                    block.Accept(this);
                    _jsonWriter.WriteEndObject();
                });
                _jsonWriter.WriteEndArray();
            }

            _jsonWriter.WriteStartArray("constants");
            proto.Constants.ForEach(constant =>
            {
                _jsonWriter.WriteStartObject();
                constant.Accept(this);
                _jsonWriter.WriteEndObject();
            });
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteStartArray("protoTypes");
            proto.ProtoTypes.ForEach(proto =>
            {
                _jsonWriter.WriteStartObject();
                proto.Accept(this);
                _jsonWriter.WriteEndObject();
            });
            _jsonWriter.WriteEndArray();

            return false;
        }

        public override bool Visit(Constant constant)
        {
            _jsonWriter.WriteString("type", constant.Type.ToString().ToLower());
            return false;
        }

        public override bool Visit(BooleanConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteBoolean("value", constant.Value);

            return false;
        }

        public override bool Visit(ClosureConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteNumber("value", constant.Value);

            return false;
        }

        public override bool Visit(ImportConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteStartArray("value");

            constant.Value.ForEach(value =>
            {
                _jsonWriter.WriteStartObject();
                value.Accept(this);
                _jsonWriter.WriteEndObject();
            });

            _jsonWriter.WriteEndArray();
            return false;
        }

        public override bool Visit(StringConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteString("value", constant.Value);

            return false;
        }

        public override bool Visit(TableConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteStartArray("value");

            constant.Value.ForEach(value =>
            {
                _jsonWriter.WriteStartObject();
                value.Accept(this);
                _jsonWriter.WriteEndObject();
            });

            _jsonWriter.WriteEndArray();
            return false;
        }

        public override bool Visit(VectorConstant constant)
        {
            Visit(constant as Constant);

            _jsonWriter.WriteStartArray("value");

            _jsonWriter.WriteNumberValue(constant.Value.X);
            _jsonWriter.WriteNumberValue(constant.Value.Y);
            _jsonWriter.WriteNumberValue(constant.Value.Z);
            _jsonWriter.WriteNumberValue(constant.Value.W);

            _jsonWriter.WriteEndArray();

            return false;
        }

        public override bool Visit(BasicBlock block)
        {
            _jsonWriter.WriteNumber("id", block.Id);
            _jsonWriter.WriteString("brachType", block.Branch!.ToString()!.ToLower());

            _jsonWriter.WriteStartArray("instructions");
            block.Instructions.ForEach(instruction =>
            {
                _jsonWriter.WriteStartObject();
                instruction.Accept(this);
                _jsonWriter.WriteEndObject();
            });
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteStartArray("outgoingEdges");
            block.OutgoingEdges.ForEach(edge =>
            {
                _jsonWriter.WriteStartObject();
                edge.Accept(this);
                _jsonWriter.WriteEndObject();
            });
            _jsonWriter.WriteEndArray();
            return false;
        }

        public override bool Visit(Edge edge)
        {
            _jsonWriter.WriteString("label", edge.Label);
            _jsonWriter.WriteNumber("target", edge.Target);

            return false;
        }

        public override bool Visit(Instruction instruction)
        {
            _jsonWriter.WriteString("opCode", instruction.Code.ToString().ToLower());
            return false;
        }

        public override bool Visit(InstructionABC instruction)
        {
            Visit(instruction as Instruction);

            _jsonWriter.WriteNumber("a", instruction.A);
            _jsonWriter.WriteNumber("b", instruction.B);
            _jsonWriter.WriteNumber("c", instruction.C);

            if (instruction.Aux != null)
                _jsonWriter.WriteNumber("aux", instruction.Aux.Value);
            return false;
        }

        public override bool Visit(InstructionAB instruction)
        {
            Visit(instruction as Instruction);

            _jsonWriter.WriteNumber("a", instruction.A);
            _jsonWriter.WriteNumber("b", instruction.B);

            if (instruction.Aux != null)
                _jsonWriter.WriteNumber("aux", instruction.Aux.Value);
            return false;
        }

        public override bool Visit(InstructionAD instruction)
        {
            Visit(instruction as Instruction);

            _jsonWriter.WriteNumber("a", instruction.A);
            _jsonWriter.WriteNumber("d", instruction.D);

            if (instruction.Aux != null)
                _jsonWriter.WriteNumber("aux", instruction.Aux.Value);
            return false;
        }

        public override bool Visit(InstructionA instruction)
        {
            Visit(instruction as Instruction);

            _jsonWriter.WriteNumber("a", instruction.A);

            if (instruction.Aux != null)
                _jsonWriter.WriteNumber("aux", instruction.Aux.Value);
            return false;
        }

        public override bool Visit(InstructionE instruction)
        {
            Visit(instruction as Instruction);

            _jsonWriter.WriteNumber("e", instruction.E);

            if (instruction.Aux != null)
                _jsonWriter.WriteNumber("aux", instruction.Aux.Value);
            return false;
        }
    }
}
