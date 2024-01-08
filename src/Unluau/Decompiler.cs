// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog;

namespace Unluau
{
    public class DecompilerOptions
    {
        public bool PerferStringInterpolation { get; set; } = true;
        public bool VariableNameGuessing { get; set; }
        public bool DescriptiveComments { get; set; }
        public bool HeaderEnabled { get; set; } = true;
        public bool InlineTableDefintions { get; set; } = false;
        public bool RenameUpvalues { get; set; }
        public string? Version { get; set; }
        public OpCodeEncoding Encoding { get; set; }
        public Output Output { get; set; } = new Output();
        public ILogger Logger { get; set; } = new LoggerConfiguration().CreateLogger();
    }

    public class Decompiler
    {
        private readonly DecompilerOptions _options;
        private readonly Chunk _chunk;

        public Guid Guid { get; private set; }

        public Decompiler(Stream stream, DecompilerOptions options)
        {
            Log.Logger = options.Logger!;

            _options = options;
            _chunk = new Deserializer(stream, options.Encoding).Deserialize();

            Guid = Guid.NewGuid();
        }

        public void Decompile()
        {
            Lifter lifter = new Lifter(_chunk, _options);

            OuterBlock program = lifter.LiftProgram();

            if (_options.HeaderEnabled)
                _options.Output.WriteLine($"-- Unluau v{_options.Version} guid: {Guid}");

            program.Write(_options.Output);
            _options.Output.Flush();
        }

        public string Dissasemble() => _chunk.ToString();
    }
}
