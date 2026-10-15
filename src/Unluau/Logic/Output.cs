// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;

namespace Unluau
{
    public class Output
    {
        private readonly TextWriter _writer;

        private int IndentationLevel { get; set; }
        private int Position { get; set; }

        public Output(TextWriter writer) => this._writer = writer;

        public Output() : this(Console.Out) {}

        public void Indent() => IndentationLevel += 3;

        public void Unindent() => IndentationLevel -= 3;

        public void Write(string value)
        {
            MoveCursor();

            _writer.Write(value);
            Position += value.Length;
        }

        public void WriteLine(string value)
        {
            MoveCursor();

            _writer.Write(value + '\n');
            Position = 0;
        }

        public void WriteLine() => WriteLine(string.Empty);

        private void MoveCursor()
        {
            if (Position != 0)
                return;

            for (int i = IndentationLevel; i != 0; i--)
            {
                _writer.Write(" ");
                Position++;
            }
        }

        public void Flush() => _writer.Flush();

        public void Join(string separator, IEnumerable<string> strings)
        {
            bool first = true;
            foreach (var str in strings)
            {
                if (!first)
                    this.Write(separator);
                
                this.Write(str);

                first = false;
            }
        }

        public void Join(string separator, IEnumerable<Expression?> expressions)
        {
            bool first = true;
            foreach (var expression in expressions.Where(e => e is not null))
            {
                if (!first)
                    this.Write(separator);
                
                expression!.Write(this);

                first = false;
            }
        }
    }
}
