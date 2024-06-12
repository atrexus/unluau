using Unluau.Common.IR;
using Unluau.Common.IR.ProtoTypes;
using Unluau.Common.IR.ProtoTypes.Constants;
using Unluau.Common.IR.ProtoTypes.Instructions;
using Unluau.Decompile.IL;
using Unluau.Decompile.IL.Blocks;
using Unluau.Decompile.IL.Conditions;
using Unluau.Decompile.IL.Instructions;
using Unluau.Decompile.IL.Values;
using Instruction = Unluau.Common.IR.ProtoTypes.Instructions.Instruction;

namespace Unluau.Decompile.Builders
{
    /// <summary>
    /// Builds IL from the IR.
    /// </summary>
    public class ILBuilder : Common.IR.Visitor
    {
        /// <summary>
        /// Builds a <see cref="Program"/> from an IR module.
        /// </summary>
        /// <param name="module">The module to build.</param>
        /// <returns>The program.</returns>
        public static Program Build(Module module)
        {
            var functions = new List<Function>();

            foreach (var protoType in module.ProtoTypes)
            {
                var builder = new ILBuilder();
                protoType.Accept(builder);

                functions.Add(builder._function!);
            }

            return new Program(functions[module.EntryPoint].Context, functions, module.EntryPoint);
        }

        private Function? _function;
        private ProtoType? _protoType;

        /// <summary>
        /// Visits a <see cref="ProtoType"/>.
        /// </summary>
        public override bool Visit(ProtoType protoType)
        {
            _function = new(_protoType = protoType)
            {
                IsEntryPoint = protoType.IsMain
            };

            // Now lets visit the instructions in the protoType
            foreach (var instruction in protoType.Instructions)
                instruction.Accept(this);

            return false;
        }

        /// <summary>
        /// Visits a <see cref="Instruction"/>.
        /// </summary>
        public override bool Visit(Instruction instruction)
        {
            var context = new IL.Context(instruction);    

            switch (instruction.Code)
            {
                case OpCode.LoadNil:
                case OpCode.LoadN:
                case OpCode.LoadB:
                case OpCode.LoadKX:
                case OpCode.LoadK:
                case OpCode.GetGlobal:
                case OpCode.GetImport:
                {
                    var value = instruction.Code switch
                    { 
                        OpCode.LoadK or OpCode.GetImport => ToBasicValue(instruction.D, context),

                        OpCode.LoadN => new BasicValue<double>(context, instruction.D),

                        _ => throw new NotSupportedException()
                    };

                    var ra = _function!.Stack.Set(instruction.A, value);

                    _function.AddInstruction(new LoadValue(context, ra, value));
                    break;
                }
                case OpCode.Call:
                {
                    var function = new Reference(context, _function!.Stack.Get(instruction.A)!);

                    List<BasicValue> arguments = [];

                    // When CALL's B operand is 0, the instruction is LUA_MULTRET. This means that its arguments start 
                    // at R(A) and go up to the top of the stack.
                    var argCount = instruction.B > 0 ? instruction.B : _function!.Stack.Top.Id - instruction.A + 1;

                    for (int i = 1; i < argCount; ++i)
                        arguments.Add(new Reference(context, _function!.Stack.Get(instruction.A + i)!));

                    // Pop the stack frame, all of the register are now "dead"
                    _function!.Stack.FreeFrame(instruction.A);

                    // Create our new call result, that we will load onto the stack.
                    var callResult = new CallResult(context, function, [.. arguments]);

                    var results = new List<Slot>();

                    // Calculate the number of return values
                    var retCount = instruction.C > 0 ? instruction.C - 1 : _function!.Stack.Top.Id - instruction.A + 1;

                    // Load all of the results onto the stack. 
                    for (int slot = 0; slot < retCount; ++slot)
                        results.Add(_function!.Stack.Set(slot + instruction.A, callResult));

                    _function.AddInstruction(new Call(context, callResult, [.. results]));
                    break;
                }
                case OpCode.Return:
                {
                    // Again this instruction uses LUA_MULTRET, just like the call instruction. If the `B` operand is empty,
                    // then we pop all the values off of the stack and use those as our values. Otherwise, `B-1` contains our
                    // value count.
                    var valueCount = instruction.B > 0 ? instruction.B - 1 : _function!.Stack.Top.Id - instruction.A + 1;

                    var values = new BasicValue[valueCount];

                    for (int i = 0; i < valueCount; ++i)
                        values[i] = new Reference(context, _function!.Stack.Get(instruction.A + i)!);

                    _function!.AddInstruction(new Return(context, [.. values]));
                    break;
                }
                case OpCode.JumpIfEq:
                {
                    var left = new Reference(context, _function!.Stack.Get(instruction.A)!);

                    var right = instruction.Code switch
                    {
                        // These operations just test a value and have one operand. 
                        OpCode.JumpIf or OpCode.JumpIfNot => null,

                        _ => new Reference(context, _function!.Stack.Get((byte)instruction.Aux!.Value)!)
                    };

                    // Build the condition based on the current operation code.
                    BasicCondition condition = instruction.Code switch
                    {
                        // Performs a jump if values are not equal to each other.
                        OpCode.JumpIfNotEq => new Equals(context, left, right!),

                        // This should never happen, but just in case.
                        _ => throw new NotImplementedException()
                    };

                    _function!.AddBlock(new IfBlock(condition));
                    break;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts an IR constant to a basic value.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <returns>The basic value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static BasicValue ToBasicValue(Constant constant, IL.Context context)
        {
            return constant switch
            {
                NilConstant => new BasicValue<object>(context, null),
                BooleanConstant boolean => new BasicValue<bool>(context, boolean.Value),
                NumberConstant number => new BasicValue<double>(context, number.Value),
                StringConstant @string => new BasicValue<string>(context, @string.Value),
                ImportConstant import => new Global(context, import.Value.Select(v => v.Value).ToArray()),
                _ => throw new InvalidOperationException("Invalid constant type.")
            };
        }

        /// <summary>
        /// Converts an IR constant to a basic value.
        /// </summary>
        /// <param name="index">The index of the constant in the constant pool.</param>
        /// <param name="context">The context about the instruction.</param>
        /// <returns>The basic value.</returns>
        private BasicValue ToBasicValue(int index, IL.Context context) => ToBasicValue(_protoType!.Constants[index], context);
    }
}
