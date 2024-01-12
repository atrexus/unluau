// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    internal class RepeatUntil : Statement
    {
        public Expression Condition { get; set; }
        public Block Body { get; set; }

        public RepeatUntil(Expression condition, Block body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.WriteLine("repeat");
            Body.Write(output);
           
            output.Write("until ");
            Condition.Write(output);
        }
    }
}
