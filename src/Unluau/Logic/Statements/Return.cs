// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;

namespace Unluau.Logic.Statements
{
    public class Return : Statement
    {
        public IList<Expression> Expressions { get; }

        public Return(IList<Expression> expressions)
            => Expressions = expressions;

        public override void Write(Output output)
        {
            output.Write("return");

            if (Expressions.Count <= 0)
                return;

            output.Write(" ");
            output.Join(", ", Expressions);
        }
    }
}
