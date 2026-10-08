// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public class Return : Statement
    {
        public IList<Expression> Expressions { get; }

        public Return(IList<Expression> expressions)
            => Expressions = expressions;

        public override void Write(Output output)
        {
            output.Write("return");
            output.Write(string.Join(", ", Expressions));
        }
    }
}
