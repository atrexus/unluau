// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public class ForLoopNumeric : Statement
    {
        public Assignment Variable { get; }
        public Expression Limit { get; }
        public Expression Step { get; }
        public Block Body { get; }

        public ForLoopNumeric(Assignment variable, Expression limit, Expression step, Block body)
        {
            Variable = variable;
            Limit = limit;
            Step = step;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.Write("for ");
            Variable.Write(output);
            output.Write(", ");
            Limit.Write(output);
            output.Write(", ");
            Step.Write(output);
            output.WriteLine(" do");
            Body.Write(output);
            output.Write("end");
        }
    }
}
