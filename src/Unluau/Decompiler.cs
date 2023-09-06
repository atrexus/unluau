// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        public bool Verbose { get; set; }
        public bool Warnings { get; set; }
        public OpCodeEncoding Encoding { get; set; }
        public Output Output { get; set; } = new Output();
        public StreamWriter? LogFile { get; set; }
    }

    public class Decompiler
    {
        private DecompilerOptions _options;
        private Chunk chunk;
        private LogManager manager;

        public Guid Guid { get; private set; }

        public Decompiler(Stream stream, DecompilerOptions options)
        {
            // Create our logger instance for the decompiler
            manager = new LogManager(GetLogSeverity(options));
            manager.LogRecieved += OnLogRecieved;

            _options = options;
            chunk = new Deserializer(manager, stream, options.Encoding).Deserialize();

            Guid = Guid.NewGuid();
        }

        private void OnLogRecieved(object? sender, LogRecievedEventArgs e)
        {
            if (e.Message.Severity == LogSeverity.Warn && !_options.Warnings)
                return;

            string Text = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{e.Message.Severity}] {e.Message.Source}: {e.Message.Exception?.ToString() ?? e.Message.Message}";

            switch (e.Message.Severity)
            {
                case LogSeverity.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            _options.LogFile!.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Decompile()
        {
            Lifter lifter = new Lifter(chunk, _options);

            OuterBlock program = lifter.LiftProgram();

            if (_options.HeaderEnabled)
                _options.Output.WriteLine($"-- Unluau v{_options.Version} guid: {Guid}");

            program.Write(_options.Output);
            _options.Output.Flush();
        }

        public string Dissasemble()
        {
            return chunk.ToString();
        }

        private LogSeverity GetLogSeverity(DecompilerOptions options)
        {
            if (options.Verbose)
                return LogSeverity.Debug;

            return LogSeverity.Info;
        }
    }
}
