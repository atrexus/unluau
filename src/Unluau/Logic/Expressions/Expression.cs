// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Diagnostics.CodeAnalysis;

namespace Unluau.Logic.Expressions
{
    public abstract class Expression
    {
        public abstract void Write(Output output);
        
        public virtual string[] GetNames() => Array.Empty<String>();

        public Expression? GetValue()
        {
            if (this is LocalExpression local)
            {
                if (local.Expression is null)
                    return null;

                return local.Expression.GetValue();
            }

            return this;
        }
    }
}
