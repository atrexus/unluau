// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class FunctionCall : Expression
    {
        public Expression Function { get; protected set; }
        public IList<Expression> Arguments { get; protected set; }

        public FunctionCall(Expression function, IList<Expression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public override void Write(Output output)
        {
            Function.Write(output);
            output.Write("(");

            bool first = true;

            foreach (Expression argument in Arguments)
            {
                if (argument is null)
                    continue;

                argument.Write(output);

                if (first)
                    first = false;
                else
                    output.Write(",");
            }

            output.Write(")");
        }

        public override string[] GetNames()
        {
            string[] names = Function.GetNames();

            if (names != null)
            {
                foreach (Expression expression in Arguments)
                {
                    string[] exprNames = expression is null ? null : expression.GetNames();

                    if (exprNames != null)
                        names = names.Concat(exprNames).ToArray();
                }
            }

            return names;
        }
    }
}
