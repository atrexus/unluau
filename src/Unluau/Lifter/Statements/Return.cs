// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Return : Statement
    {
        public IList<Expression> Expressions { get; protected set; }

        public Return(IList<Expression> expressions)
            => Expressions = expressions;

        public Return()
            : this(new List<Expression>())
        { }

        public override void Write(Output output)
        {
            output.Write("return ");

            bool first = true;

            foreach (Expression expression in Expressions)
            {
                if (expression is null)
                    continue;

                expression.Write(output);

                if (first)
                    first = false;
                else
                    output.Write(",");
            }
        }
    }
}
