// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Utilities
{
    public enum Stage
    {
        Deserializer,
        Lifter
    }

    public class DecompilerException : Exception
    {
        public Stage Stage { get; init; }

        public DecompilerException(Stage stage, string message)
            : base(message)
        {
            Stage = stage;
        }
    }
}
