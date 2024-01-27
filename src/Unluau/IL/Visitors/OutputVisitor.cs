using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //Writer.WriteLine($"{node.Parameters.Length}{(node.IsVariadic ? '+' : string.Empty)} param(s)");

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

            Writer.WriteLine($"){string.Format("{0, 57}", $"-- {node.Context}")}");

            return true;
        }

        public override bool Visit(LoadValue node)
        {
            Writer.Write(string.Format("{0, -10} {1, -8}  {2, -30} -- {3}", $"LoadValue", $"R({node.Slot})", node.Value.ToString(), node.Context.ToString()));

            return false;
        }

        public override bool Visit(Call node)
        {
            Writer.Write(string.Format("{0, -10} {1, -8}  {2, -30} -- {3}", $"Call", node.Callee.ToString(), TypeExtentions.ToString(node.Arguments), node.Context.ToString()));
            
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
    }
}
