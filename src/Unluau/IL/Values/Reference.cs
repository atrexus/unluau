using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL.Values
{
    /// <summary>
    /// A reference to a register slot in the current function.
    /// </summary>
    /// <param name="context">Information about the refrence.</param>
    /// <param name="slot">A register slot.</param>
    public class Reference(Context context, Slot slot) : BasicValue(context)
    {
        /// <summary>
        /// A register slot in memory.
        /// </summary>
        public Slot Slot { get; set; } = slot;

        /// <summary>
        /// Converts the curren <see cref="Reference"/> to a string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string? ToString() => $"Ref({Slot})";

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
