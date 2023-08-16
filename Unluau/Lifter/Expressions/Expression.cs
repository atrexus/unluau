// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public abstract class Expression
    {
        public abstract void Write(Output output);
        public virtual string[] GetNames()
        {
            return null;
        }
    }
}
