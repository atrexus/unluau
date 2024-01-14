// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class ForLoopGeneric : Statement
    {
        public List<Decleration> Variables { get; set; }
        public ExpressionList Values { get; set; }
        public Block Body { get; set; }

        public ForLoopGeneric(List<Decleration> variables, ExpressionList values, Block body)
        {
            Variables = variables;
            Values = values;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.Write("for ");

            bool first = true;

            foreach (var decl in Variables)
            {
                if (!first) 
                { 
                    output.Write(", "); 
                }

                output.Write($"{decl.Name}");

                first = false;
            }

            output.Write(" in ");
            Values.Write(output);
            output.WriteLine(" do");

            Body.Write(output);
            output.Write("end");
        }
    }
}
