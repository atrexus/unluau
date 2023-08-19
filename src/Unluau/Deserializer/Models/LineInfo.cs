// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LineInfo
    {
        public byte LineGapLog { get; set; }
        public IList<byte> LineInfoList { get; set; }
        public IList<int> AbsLineInfoList { get; set; }
    }
}
