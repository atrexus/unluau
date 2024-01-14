// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class ExpressionList : Expression
    {
        public IList<Expression> Expressions { get; set; }

        public ExpressionList(int count)
            => Expressions = new List<Expression>(count);

        public void Append(Expression expression)
            => Expressions.Add(expression);

        public override void Write(Output output)
        {
            for (int i = 0; i < Expressions.Count; ++i)
            {
                if (Expressions[i] == null)
                    continue;

                if (i != 0)
                    output.Write(", ");

                Expressions[i].Write(output);
            }
        }
    }
}
