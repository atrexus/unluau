// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class VectorLiteral : Expression
    {
        public Vector4 Vec { get; set; }

        public VectorLiteral(Vector4 vec)
        {
            Vec = vec;
        }

        public override void Write(Output output)
        {
            // Vector3 seems to be the only builtin type used for vector constant, yet vector constant holds 4 floats.
            output.Write($"Vector3.new({Vec.X}, {Vec.Y}, {Vec.Z})");
        }
    }
}
