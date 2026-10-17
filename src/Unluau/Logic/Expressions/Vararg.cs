// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class Vararg : Expression
    {
        public override void Write(Output output)
        {
            output.Write("...");
        }
    }
}
