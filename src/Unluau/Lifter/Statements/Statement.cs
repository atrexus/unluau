// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public abstract class Statement
    {
        public string? Comment { get; set; }

        public abstract void Write(Output output);

        public static void WriteSequence(Output output, IList<Statement> statements)
        {
            for (int i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];
                if (statement.Comment != null)
                    output.WriteLine("-- " + statement.Comment);

                statement.Write(output);
                output.WriteLine();
            }
        }
    }
}
