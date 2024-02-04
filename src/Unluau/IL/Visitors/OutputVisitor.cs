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
            Writer.Write(Format(node.Context, $"LoadValue", node.Slot.ToString(), node.Value.ToString()));

            return false;
        }

        public override bool Visit(Call node)
        {
            Writer.Write(Format(node.Context, $"Call", node.CallResult.ToString(), $"Ret({TypeExtentions.ToString(node.Slots)})"));
            
            return false;
        }

        public override bool Visit(GetIndexSelf node)
        {
            Writer.Write(Format(node.Context, $"GetIndexSelf", node.Slot.ToString(), node.Index.ToString()));

            return false;
        }

        public override bool Visit(Move node)
        {
            Writer.Write(Format(node.Context, $"Move", node.Target.ToString(), node.Source.ToString()));

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

            stringBuilder.Append(string.Format("{0, -10} {1, -15} {2, -8} {3, -30} {4, -14}", context, op, a, b, c));

            return stringBuilder.ToString();
        }

        private static string Format(Context context, string op, string? a, string? b)
            => Format(context, op, a, b, string.Empty);
    }
}
