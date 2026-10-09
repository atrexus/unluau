// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
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

            if (names.Length <= 0)
                return names;

            foreach (Expression expression in Arguments)
            {
                if (expression is not LocalExpression { Expression: not null } local)
                    continue;

                names = names.Concat(local.Expression.GetNames()).ToArray();
            }

            return names;
        }
    }
}
