using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// Concatenates values in registers.
    /// </summary>
    /// <param name="context">Information about the location.</param>
    /// <param name="registers">The registers that are concatenated together.</param>
    public class Concat(Context context, BasicValue[] registers) : BasicValue(context)
    {
        /// <summary>
        /// The registers that are concatenated together.
        /// </summary>
        public BasicValue[] Registers { get; set; } = registers;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var reg in Registers)
                {
                    reg.Visit(visitor);
                }
            }
        }

        /// <summary>
        /// Converts the current <see cref="Concat"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() => $"Concat({TypeExtensions.ToString(Registers)})";
    }
}
