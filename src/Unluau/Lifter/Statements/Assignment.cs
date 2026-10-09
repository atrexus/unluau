// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public class Assignment : Statement
    {
        public Expression? Variable { get; private set; }
        public Expression? Value { get; private set; }

        public Assignment(Expression? variable, Expression? value)
        {
            Variable = variable;
            Value = value;
        }

        public override void Write(Output output)
        {
            if (Variable == null || Value == null)
            {
                output.Write("-- [Unluau: Failed to decompile assignment]");
                return;
            }
    
            if (Value is LocalExpression { Expression: Closure } expression)
            {
                output.Write("function ");
                Variable.Write(output);

                expression.Expression.Write(output);
                output.WriteLine();
            }
            else
            {
                Variable.Write(output);
                output.Write(" = ");
                Value.Write(output);
            }
        }
    }
}
