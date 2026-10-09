// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Statements;

namespace Unluau.Logic.Expressions
{
    public class Closure : Expression
    {
        public IList<Declaration> Arguments { get; protected set; }
        public bool HasVararg { get; protected set; }
        public Block Block { get; protected set; }
        public int FunctionId { get; protected set; }

        public Closure(IList<Declaration> arguments, bool hasVararg, Block block, int fId)
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
            foreach (Declaration argument in Arguments)
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

            // Add null check for Block
            if (Block != null)
                Block.Write(output);
            else
                output.WriteLine("-- [Unluau: Missing function body]");

            output.Write("end");
        }
    }
}
