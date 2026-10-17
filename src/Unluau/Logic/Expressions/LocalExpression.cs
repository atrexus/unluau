// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class LocalExpression : Expression
    {
        public Expression? Expression { get; private set; }
        public Declaration Declaration { get; set; }

        public LocalExpression(Expression? expression, Declaration declaration)
        {
            Expression = expression;
            Declaration = declaration;
        }

        public override void Write(Output output)
        {
            if (Declaration.Referenced != 1)
            {
                output.Write(Declaration.Name);
                return;
            }
            
            if (Expression != null)
            {
                if (Expression is Closure)
                    output.Write("function");

                Expression.Write(output);
                return;
            }
            
            output.Write(Declaration.Name);
        }

        public override string[] GetNames()
        {
            if (Expression is null)
                return new[] { Declaration.Name };

            return Expression.GetNames();
        }
    }
}
