using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class TableLiteral : Expression
    {
        public sealed class Entry
        {
            public Expression Key { get; set; }
            public Expression Value { get; set; }

            public Entry(Expression key, Expression value)
            {
                Key = key;
                Value = value;
            }
        }

        public IList<Entry> Entries { get; set; }
        public bool IsList { get; set; }
        public bool IsDictionary { get; set; }
        
        // Used for inlining table definitions
        public int MaxEntries { get; set; }

        public TableLiteral(int size, bool isList)
        {
            Entries = new List<Entry>(size);

            IsList = isList;
            IsDictionary = !isList;
        }

        public void AddEntry(Entry entry)
        {
            Entries.Add(entry);
        }

        public override void Write(Output output)
        {
            output.Write("{ ");

            bool itemsOnNewline = (IsList && Entries.Count > 10) || (IsDictionary && Entries.Count > 5);
            bool isFirst = true;

            if (itemsOnNewline)
                output.Indent();

            foreach (Entry entry in Entries)
            {
                if (!isFirst)
                    output.Write(", ");
                else
                    isFirst = false;

                if (itemsOnNewline)
                    output.WriteLine();

                if (entry.Key != null)
                {
                    if (entry.Key is NumberLiteral || entry.Key is StringLiteral)
                    {
                        output.Write("[");
                        entry.Key.Write(output);
                        output.Write("]");
                    }
                    else
                        entry.Key.Write(output);

                    output.Write(" = ");
                }

                entry.Value.Write(output);
            }

            if (itemsOnNewline)
            {
                output.Unindent();
                output.WriteLine();
                output.Write("}");
            }
            else
                output.Write(" }");
        }
    }
}
