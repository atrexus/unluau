// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Local
    {
        public string Name { get; protected set; }
        public Local Shadow { get; protected set; }

        public Local(string name, Local shadow = null)
        {
            Name = name;
            Shadow = shadow;
        }
    }
}
