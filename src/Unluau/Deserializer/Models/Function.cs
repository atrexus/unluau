// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unluau
{
    public class Function
    {
        public int Id { get; set; }
        public byte MaxStackSize { get; set; }
        public byte Parameters { get; set; }
        public byte MaxUpvalues { get; set; }
        public IList<LocalExpression> Upvalues { get; set; }
        public bool IsVararg { get; set; }
        public byte Flags { get; set; }
        public byte[] Types { get; set; }
        public int LineDefined { get; set; }
        public string DebugName { get; set; }
        public IList<Constant> Constants { get; set; }
        public IList<Instruction> Instructions { get; set; }
        public IList<int> Functions { get; set; }
        public LineInfo? LineInfo { get; set; }
        public DebugInfo? DebugInfo { get; set; }

        public IList<Function> GlobalFunctions { get; set; }

        public Function GetFunction(int fId)
            => GlobalFunctions[Functions[fId]];

        public Constant GetConstant(int pc)
            => Constants[Convert.ToInt32(Instructions[pc].Value)];

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Parameters}{(IsVararg ? "+" : string.Empty)} param(s), {MaxStackSize} slot(s), {MaxUpvalues} upvalue(s), {Constants.Count} constant(s), {Functions.Count} function(s)\n");

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
                }

                if (instruction.GetProperties().HasAux)
                {
                    builder.Append((i + 1).ToString("000"));
                    builder.Append(string.Format("   {0, -10}\t {1, 5}\n", "   AUX", (int)Instructions[++i].Value));
                }
            }

            if (DebugInfo != null)
                builder.Append("\n" + DebugInfo.ToString());

            // Display constants
            builder.Append($"\n   constants ({Constants.Count})\n");

            for (int i = 0; i < Constants.Count; i++)
            {
                Constant constant = Constants[i];

                var format = string.Format("      {0, -10} {1, -10} {2, -10}", i + 1, constant.Type.ToString().ToLower(), constant);
                builder.Append(format + "\n");
            }

            builder.Append($"end -- function id: {Id}\n");

            return builder.ToString();
        }
    }
}
