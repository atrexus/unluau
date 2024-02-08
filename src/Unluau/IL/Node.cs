using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL
{
    /// <summary>
    /// Contains information about location. 
    /// </summary>
    public struct Context((int, int) pcScope, (int, int)? lines = null)
    {
        /// <summary>
        /// The empty context for a node in the IL.
        /// </summary>
        public static Context Empty { get; } = new();

        /// <summary>
        /// The start and end instruction the node was translated from.
        /// </summary>
        public (int, int) PcScope { get; set; } = pcScope;

        /// <summary>
        /// The line numbers from the original script.
        /// </summary>
        public (int, int)? Lines { get; set; } = lines;

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
