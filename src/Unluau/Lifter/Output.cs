// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Output
    {
        private TextWriter writer;

        private int indentationLevel = 0;
        private int position = 0;

        public int IndentationLevel {
            get { return indentationLevel; }
            set { indentationLevel = value; }
        }

        public int Position {
            get { return position; }
        }

        public bool SpaceChildren { get; set; }

        public Output(TextWriter writer)
            => this.writer = writer;

        public Output()
            : this(Console.Out)
        { }

        public void Indent()
        {
            indentationLevel += 3;
        }

        public void Unindent()
        {
            indentationLevel -= 3; 
        }

        public void Write(string value)
        {
            MoveCursor();

            writer.Write(value);
            position += value.Length;
        }

        public void WriteLine(string value)
        {
            MoveCursor();

            writer.Write(value + '\n');
            position = 0;
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        private void MoveCursor()
        {
            if (position == 0)
            {
                for (int i = indentationLevel; i != 0; i--)
                {
                    writer.Write(" ");
                    position++;
                }
            }
        }

        public void Flush()
        {
            writer.Flush();
        }
    }
}
