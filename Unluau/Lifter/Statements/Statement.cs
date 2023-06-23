// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public abstract class Statement
    {
        public string Comment { get; set; }

        public abstract void Write(Output output);

        public static void WriteSequence(Output output, IList<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                if (statement.Comment != null)
                    output.WriteLine("-- " + statement.Comment);

                statement.Write(output);
                output.WriteLine();
            }
        }
    }
}
