using System.Text;
using Unluau.IL.Statements;
using Unluau.IL.Statements.Blocks;
using Unluau.IL.Statements.Instructions;
using Unluau.Utils;

namespace Unluau.IL.Visitors
{
    public class OutputVisitor(Stream stream) : Visitor
    {
        public readonly StreamWriter Writer = new(stream) { AutoFlush = true };

        private static int Indent = 0;

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
            Writer.Write(Format(node.Context, $"Call", node.CallResult.ToString(), $"Ret({TypeExtensions.ToString(node.Slots)})"));

            return false;
        }

        public override bool Visit(GetIndexSelf node)
        {
            Writer.Write(Format(node.Context, $"GetIndexSelf", node.Slot.ToString(), node.Index.ToString()));

            return false;
        }

        public override bool Visit(GetIndex node)
        {
            Writer.Write(Format(node.Context, $"GetIndex", node.Slot.ToString(), node.Index.ToString()));

            return false;
        }

        public override bool Visit(Move node)
        {
            Writer.Write(Format(node.Context, $"Move", node.Target.ToString(), node.Source.ToString()));

            return false;
        }

        public override bool Visit(BasicBlock node)
        {
            Writer.WriteLine(Format(node.Context, "BasicBlock {"));

            Indent++;
            WriteStatements(node.Statements);
            Indent--;

            Writer.Write(Format(node.Context, "}"));

            return false;
        }

        public override bool Visit(IfBlock node)
        {
            Writer.Write(Format(node.Context, $"IfBlock {node.Condition}"));
            Writer.WriteLine(" {");

            Indent++;
            WriteStatements(node.Statements);
            Indent--;

            Writer.Write(Format(node.Context, "}"));
            return false;
        }

        private static string Format(Context context, string op, string? a, string? b, string? c)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append(string.Format(" {0, -15} {1, -15} {2, -8} {3, -30} {4, -14}", context, new string(' ', Indent * 2) + op, a, b, c));

            return stringBuilder.ToString();
        }

        private static string Format(Context context, string op, string? a, string? b)
            => Format(context, op, a, b, " ");

        private static string Format(Context context, string op)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append(string.Format(" {0, -15} {1}", context, new string(' ', Indent * 2) + op));

            return stringBuilder.ToString();
        }

        private void WriteStatements(List<Statement> statements)
        {
            foreach (var statement in statements)
            {
                statement.Visit(this);
                Writer.Write('\n');
            }
        }
    }
}
