// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalExpression : Expression
    {
        public Expression Expression { get; private set; }
        public Declaration Declaration { get; set; }

        public LocalExpression(Expression expression, Declaration declaration)
        {
            Expression = expression;
            Declaration = declaration;
        }

        public override void Write(Output output)
        {
            if (Declaration.Referenced != 1)
                output.Write(Declaration.Name);
            else if (Expression != null)
            {
                if (Expression is Closure)
                    output.Write("function");

                Expression.Write(output);
            }
            else
                output.Write(Declaration.Name);
        }

        public override string[] GetNames()
        {
            if (Expression is null)
                return new string[] { Declaration.Name };

            return Expression.GetNames();
        }
    }
}
