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

                if (first)
                    first = false;
                else
                    output.Write(", ");

                argument.Write(output);
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
                    // If expression is null we can't get names from it
                    if (expression is null)
                        continue;

                    if (expression is LocalExpression local && local.Expression != null)
                    {
                        string[] expressionNames = local.Expression.GetNames();

                        if (expressionNames != null)
                            names = names.Concat(expressionNames).ToArray();
                    }          
                }
            }

            return names;
        }
    }
}
