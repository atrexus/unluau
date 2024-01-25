using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL.Values
{
    /// <summary>
    /// A new variable that doesn't have a known value.
    /// </summary>
    /// <param name="context">Additional information about location.</param>
    /// <param name="slot">The register slot the variable lives in.</param>
    /// <param name="symbol">The optional variable name.</param>
    public class Variable(Context context, int slot, string? symbol = null) : BasicValue(context)
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string? Symbol { get; set; } = symbol;

        /// <summary>
        /// The assigned register slot for the variable.
        /// </summary>
        public int Slot { get; set; } = slot;

        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
