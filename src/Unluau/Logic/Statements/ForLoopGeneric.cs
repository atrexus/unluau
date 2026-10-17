// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;

namespace Unluau.Logic.Statements
{
    public class ForLoopGeneric : Statement
    {
        public List<Declaration> Variables { get; }
        public ExpressionList Values { get; }
        public Block Body { get; }

        public ForLoopGeneric(List<Declaration> variables, ExpressionList values, Block body)
        {
            Variables = variables;
            Values = values;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.Write("for ");
            
            output.Join(", ", Variables.Select(d => d.Name));

            output.Write(" in ");
            Values.Write(output);
            output.WriteLine(" do");

            Body.Write(output);
            output.Write("end");
        }
    }
}
