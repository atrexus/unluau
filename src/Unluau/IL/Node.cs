using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.Common.IR.ProtoTypes;

namespace Unluau.Decompile.IL
{
    /// <summary>
    /// Contains information about location. 
    /// </summary>
    public struct Context
    {
        /// <summary>
        /// Creates a new instance of <see cref="Context"/>.
        /// </summary>
        /// <param name="instructions">List of instructions.</param>
        public Context(List<Instruction> instructions)
        {
            PcScope = (0, instructions.Count - 1);
            
            var first = instructions[0];
            var last = instructions[^1];

            if (first.LineDefined != null && last.LineDefined != null)
                Lines = (first.LineDefined.Value, last.LineDefined.Value);
            else 
                Lines = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Context"/>.
        /// </summary>
        /// <param name="pcScope">The scope of program counters.</param>
        /// <param name="lines">The scope in lines.</param>
        public Context((int, int) pcScope, (int, int)? lines = null)
        {
            PcScope = pcScope;
            Lines = lines;
        }

        /// <summary>
        /// The empty context for a node in the IL.
        /// </summary>
        public static Context Empty { get; } = new();

        /// <summary>
        /// The start and end instruction the node was translated from.
        /// </summary>
        public (int, int) PcScope { get; set; }

        /// <summary>
        /// The line numbers from the original script.
        /// </summary>
        public (int, int)? Lines { get; set; }

        public static bool operator ==(Context left, Context right) => Equals(left, right);
        public static bool operator !=(Context left, Context right) => !Equals(left, right);

        public override readonly string ToString()
            => $"<{PcScope.Item1}:{PcScope.Item2}{(Lines is null ? string.Empty : $",{Lines.Value.Item1}")}>";

        public override readonly bool Equals(object? obj)
        {
            if (obj is Context context)
                return context.PcScope == PcScope && Lines == context.Lines;

            return false;
        }

        public override readonly int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// The base node for all IL components.
    /// </summary>
    /// <param name="context">Additional context on the node.</param>
    public class Node(Context context)
    {
        /// <summary>
        /// Provides additional context on the current node.
        /// </summary>
        public Context Context { get; set; } = context;

        /// <summary>
        /// Recursive visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public virtual void Visit(Visitor visitor)
        {
            return;
        }
    }
}
