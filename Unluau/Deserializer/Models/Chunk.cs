// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Chunk
    {
        public IList<Function> Functions { get; set; }
        public int MainIndex { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach(Function function in Functions)
            {
                builder.Append(function.ToString() + "\n");
            }

            builder.Append("Main Function: " + MainIndex);
            return builder.ToString();
        }
    }
}
