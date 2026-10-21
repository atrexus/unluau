// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Statements
{
    public abstract class Statement
    {
        public string? Comment { get; set; }

        public abstract void Write(Output output);

        protected static void WriteSequence(Output output, IList<Statement> statements)
        {
            foreach (var statement in statements)
            {
                if (statement.Comment != null)
                    output.WriteLine("-- " + statement.Comment);

                statement.Write(output);
                output.WriteLine();
            }
        }
    }
}
