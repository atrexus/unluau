// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class WhileLoop : Statement
    {
        public Expression Condition { get; set; }
        public Block Body { get; set; }

        public WhileLoop(Expression condition, Block body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.Write("while ");
            Condition.Write(output);
            output.WriteLine(" do");

            Body.Write(output);

            output.Write("end");
        }
    }
}
