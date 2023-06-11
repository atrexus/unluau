// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalAssignment : Statement
    {
        public LocalExpression Variable { get; private set; }

        public LocalAssignment(LocalExpression variable)
            => Variable = variable;

        public override void Write(Output output)
        {
            output.Write("local ");

            if (Variable.Expression is Closure)
            {
                output.Write("function ");
                Variable.Write(output);
                Variable.Expression.Write(output);
            }
            else
            {
                Variable.Write(output);
                output.Write(" = ");
                Variable.Expression.Write(output);
            }
        }
    }
}
