// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public enum BytecodeTypes
    {
        Nil = 0,
        Boolean,
        Number,
        String,
        Table,
        Function,
        Thread,
        UserData,
        Vector,

        Any = 15,
        OptionalBit = 1 << 7,

        Invalid = 256
    };
}
