// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;

namespace Unluau.Logic.Statements
{
    public class IfElse : Statement
    {
        public Expression? Condition { get; set; }
        public Block IfBody { get; set; }
        public Statement? ElseBody { get; set; }

        public IfElse(Expression? condition, Block ifBody)
        {
            Condition = condition;
            IfBody = ifBody;
            ElseBody = null;
        }

        public override void Write(Output output)
        {
            if (Condition == null)
            {
                output.WriteLine("-- [Unluau: Failed to decompile if statement - missing condition or body]");
                return;
            }
    
            output.Write("if ");
            Condition.Write(output);
            output.WriteLine(" then");

            IfBody.Write(output);

            if (ElseBody != null)
            {
                bool elseBlock = ElseBody is Block;

                output.Write("else");

                if (elseBlock)
                    output.WriteLine();

                ElseBody.Write(output);

                if (elseBlock)
                    output.Write("end");
            }
            else
                output.Write("end");
        }
    }
}
