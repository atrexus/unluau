// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Text;
using Unluau.Logic;
using Unluau.Logic.Expressions;

namespace Unluau.Format.Models
{
    public class Function
    {
        public int Id { get; init; }
        public byte MaxStackSize { get; init; }
        public byte Parameters { get; init; }
        public byte MaxUpvalues { get; init; }
        public IList<LocalExpression> Upvalues { get; init; } = new List<LocalExpression>();
        public bool IsVararg { get; init; }
        public byte Flags { get; set; }
        public int LineDefined { get; set; }
        public string DebugName { get; init; } = "";
        public IList<Constant> Constants { get; init; } = new List<Constant>();
        public IList<Instruction> Instructions { get; init; } = new List<Instruction>();
        public IList<int> Functions { get; init; } = new List<int>();
        public LineInfo? LineInfo { get; init; }
        public DebugInfo? DebugInfo { get; init; }

        public IList<Function> GlobalFunctions { get; init; } = new List<Function>();

        public Function GetFunction(int fId)
            => GlobalFunctions[Functions[fId]];

        public Constant GetConstant(int pc)
            => Constants[Convert.ToInt32(Instructions[pc].Value)];

        public T GetConstantOrThrow<T>(int constantIndex) where T : Constant
        {
            var constant = Constants[constantIndex];
            if (constant is not T typedConstant)
                throw new ArgumentException("Constant value of wrong type; expected" +
                                            $" {typeof(T).FullName} but got {constant.GetType().FullName}");
            return typedConstant;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Parameters}{(IsVararg ? "+" : string.Empty)} param(s), ");
            builder.Append($"{MaxStackSize} slot(s), ");
            builder.Append($"{MaxUpvalues} upvalue(s), ");
            builder.Append($"{Constants.Count} constant(s), ");
            builder.Append($"{Functions.Count} function(s)\n");

            builder.Append($"function {(string.IsNullOrEmpty(DebugName) ? "main" : DebugName)}(");

            for (int i = 0; i < Parameters; i++)
            {
                builder.Append($"v{i + 1}");

                if (i + 1 < Parameters)
                    builder.Append(", ");
            }

            if (IsVararg)
            {
                if (Parameters != 0)
                    builder.Append(", ");
                builder.Append("...");
            }

            builder.Append($"){(LineInfo != null ? $" -- line {LineInfo.LineInfoList.First()} through {LineInfo.LineInfoList.Last()}" : string.Empty)}\n");

            for (int i = 0; i < Instructions.Count; ++i)
            {
                Instruction instruction = Instructions[i];

                builder.Append(i.ToString("000"));

                switch (instruction.GetProperties().Mode)
                {
                    case OpMode.iABC:
                        builder.Append(string.Format("   {0, -10}\t {1, 5} {2} {3}\n", instruction.GetProperties().Code.ToString(),
                            instruction.A, instruction.B, instruction.C));
                        break;
                    case OpMode.iAD:
                        builder.Append(string.Format("   {0, -10}\t {1, 5} {2}\n", instruction.GetProperties().Code.ToString(),
                            instruction.A, instruction.D));
                        break;
                    case OpMode.iE:
                        builder.Append(string.Format("   {0, -10}\t {1, 5}\n", instruction.GetProperties().Code.ToString(),
                            instruction.E));
                        break;
                    case OpMode.iAB:
                        builder.Append(string.Format("   {0, -10}\t {1, 5} {2}\n", instruction.GetProperties().Code.ToString(),
                            instruction.A, instruction.B));
                        break;
                    case OpMode.iA:
                        builder.Append(string.Format("   {0, -10}\t {1, 5}\n", instruction.GetProperties().Code.ToString(),
                            instruction.A));
                        break;
                    case OpMode.iD:
                        builder.Append(string.Format("   {0, -10}\t {1, 5}\n", instruction.GetProperties().Code.ToString(),
                            instruction.D));
                        break;
                }

                if (instruction.GetProperties().HasAux)
                {
                    builder.Append((i + 1).ToString("000"));
                    builder.Append(string.Format("   {0, -10}\t {1, 5}\n", "   AUX", (int)Instructions[++i].Value));
                }
            }

            if (DebugInfo != null)
                builder.Append("\n" + DebugInfo);

            // Display constants
            builder.Append($"\n   constants ({Constants.Count})\n");

            for (int i = 0; i < Constants.Count; i++)
            {
                Constant constant = Constants[i];

                var format = string.Format("      {0, -10} {1, -10} {2, -10}", i, constant.Type.ToString().ToLower(), constant);
                builder.Append(format + "\n");
            }

            builder.Append($"end -- function id: {Id}\n");

            return builder.ToString();
        }
        
        public Registers CreateRegisters(DecompilerOptions options)
        {
            IDictionary<int, Declaration> declarations = LoadDeclarations();
            var expressions = declarations
                .ToDictionary(kp => kp.Key, kp => new LocalExpression(null, kp.Value));
            
            return new Registers(declarations, expressions, options);
        }
        
        private IDictionary<int, Declaration> LoadDeclarations()
        {
            var declarations = new Dictionary<int, Declaration>();

            if (DebugInfo != null)
            {
                // Load the entire debug info into the declarations
                foreach (LocalVariable variable in DebugInfo.Locals)
                    declarations[variable.Slot] = new Declaration(variable);
            }
            else
            {
                for (int slot = 0; slot < Parameters; slot++)
                    declarations[slot] = new Declaration(slot, "arg" + (slot + 1), 0);
            }

            return declarations;
        }
    }
}
