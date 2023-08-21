// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Closure : Expression
    {
        public IList<Decleration> Arguments { get; protected set; }
        public bool HasVararg { get; protected set; }
        public Block Block { get; protected set; }
        public int FunctionId { get; protected set; }

        public Closure(IList<Decleration> arguments, bool hasVararg, Block block, int fId)
        {
            Arguments = arguments;
            HasVararg = hasVararg;
            Block = block;
            FunctionId = fId;
        }

        public override void Write(Output output)
        {
            output.Write("(");

            bool first = true;
            foreach (Decleration argument in Arguments)
            {
                if (!first)
                    output.Write(", ");
                else
                    first = false;

                output.Write(argument.Name);
            }

            if (HasVararg)
                output.Write($"{(Arguments.Count > 0 ? ", " : "")}...");

            output.WriteLine(")");

            Block.Write(output);

            output.Write("end");
        }
    }
}
