// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class ExpressionIndex : Expression
    {
        public Expression Expression { get; set; }
        public Expression Index { get; set; }

        public ExpressionIndex(Expression expression, Expression index)
        {
            Expression = expression;
            Index = index;
        }

        public override void Write(Output output)
        {
            Expression.Write(output);

            output.Write("[");
            Index.Write(output);
            output.Write("]");
        }
    }
}
