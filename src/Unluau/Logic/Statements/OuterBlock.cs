// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Statements
{
    public class OuterBlock : Block
    { 
        public OuterBlock(List<Statement> statements)
            : base(statements)
        { }

        public OuterBlock(Block block)
            : base(block.Statements)
        { }

        public override void Write(Output output)
        {
            WriteSequence(output, Statements);
        }
    }
}
