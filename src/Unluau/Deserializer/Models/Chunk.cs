// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Text;

namespace Unluau
{
    public class Chunk
    {
        public IList<Function> Functions { get; }
        public int MainIndex { get; }

        public Chunk(IList<Function> functions, int mainIndex)
        {
            this.Functions = functions;
            this.MainIndex = mainIndex;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendJoin("\n", Functions);
            builder.Append("\n");

            builder.Append("Main Function: " + MainIndex);
            return builder.ToString();
        }
    }
}
