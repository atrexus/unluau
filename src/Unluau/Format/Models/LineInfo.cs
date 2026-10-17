// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Format.Models
{
    public class LineInfo
    {
        public byte LineGapLog { get; set; }
        public IList<byte> LineInfoList { get; set; }
        public IList<int> AbsLineInfoList { get; set; }
        
        public LineInfo(IList<byte> lineInfoList, IList<int> absLineInfoList)
        {
            LineInfoList = lineInfoList;
            AbsLineInfoList = absLineInfoList;
        }
    }
}
