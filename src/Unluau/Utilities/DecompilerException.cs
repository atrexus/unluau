// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public enum Stage
    {
        Deserializer,
        Lifter
    }

    public class DecompilerException : Exception
    {
        public Stage Stage { get; private set; }
        public DecompilerException() { }

        public DecompilerException(Stage stage, string message)
            : base(message)
        {
            Stage = stage;
        }

        public static void Assert(bool condition, Stage stage)
        {
            if (!condition)
                throw new DecompilerException(stage, "Assetation failed");
        }
    }
}
