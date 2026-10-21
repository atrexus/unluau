// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public abstract class Expression
    {
        public abstract void Write(Output output);
        
        public virtual string[] GetNames() => Array.Empty<String>();

        public Expression? GetValue()
        {
            return this is not LocalExpression local ? this : local.Expression?.GetValue();
        }

        public Expression InvertCondition()
        {
            if (this is UnaryExpression unaryExpression)
                return unaryExpression.Invert();
            
            return new UnaryExpression(this, UnaryExpression.UnaryOperation.Not);
        }
    }
}
