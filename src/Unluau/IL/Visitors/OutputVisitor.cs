using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unluau.IL.Blocks;
using Unluau.IL.Instructions;
using Unluau.IL.Values;
using Unluau.Utils;

namespace Unluau.IL.Visitors
{
    public class OutputVisitor(Stream stream) : Visitor
    {
        public readonly StreamWriter Writer = new(stream) { AutoFlush = true };

        public override bool Visit(Closure node)
        {
            Writer.Write($"Closure {(node.IsMain ? "main" : node.Name)}(");

            for (int i = 0; i < node.Parameters.Length; i++)
            {
                var variable = node.Parameters[i];

                if (i > 0)
                    Writer.Write(", ");

                Writer.Write($"{(variable.Symbol is null ? $"v{variable.Slot}" : variable.Symbol)}");
            }

            if (node.IsVariadic)
            {
                if (node.Parameters.Length > 0)
                    Writer.Write(", ");

                Writer.Write("...");
            }

            Writer.WriteLine($"){string.Format(" {0}", $"-- {node.Context}")}");

            return true;
        }

        public override bool Visit(LoadValue node)
        {
            Writer.Write(Format(node.Context, $"LoadValue", $"R({node.Slot})", node.Value.ToString()));

            return false;
        }

        public override bool Visit(Call node)
        {
            Writer.Write(Format(node.Context, $"Call", node.Callee.ToString(), TypeExtentions.ToString(node.Arguments), $"Ret({node.Results})"));
            
            return false;
        }

        public override bool Visit(GetIndexSelf node)
        {
            Writer.Write(Format(node.Context, $"GetIndexSelf", $"R({node.Slot})", node.Indexable.ToString(), node.Index.ToString()));

            return false;
        }

        public override bool Visit(BasicBlock node)
        {
            foreach (var instruction in node.Instructions)
            {
                Writer.Write("  ");
                instruction.Visit(this);
                Writer.Write('\n');
            }

            return false;
        }

        private static string Format(Context context, string op, string? a, string? b, string? c)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append($"{context}  ");
            stringBuilder.Append(string.Format("{0, -15} {1, -8} {2, -30} {3, -14}", op, a, b, c));

            return stringBuilder.ToString();
        }

        private static string Format(Context context, string op, string? a, string? b)
            => Format(context, op, a, b, string.Empty);
    }
}
