// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalVariable
    {
        public string Name { get; set; }
        public int StartPc { get; set; }
        public int EndPc { get; set; }
        public byte Slot { get; set; }
    }

    public class DebugInfo
    {
        public IList<LocalVariable> Locals { get; set; }
        public IList<string> Upvalues { get; set; }

        public override string ToString()
        {
            return "DebugInfo enabled.";
        }
    }
}
