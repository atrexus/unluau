using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// Contains the result of an object call with the provided parameters and arguments.
    /// </summary>
    /// <param name="context">Information about the value.</param>
    /// <param name="callee">The function getting called.</param>
    /// <param name="arguments">The arguments to the call.</param>
    public class CallResult(Context context, BasicValue callee, BasicValue[] arguments) : BasicValue(context)
    {
        /// <summary>
        /// The object that gets called.
        /// </summary>
        public BasicValue Callee { get; set; } = callee;

        /// <summary>
        /// Arguments to the object call.
        /// </summary>
        public BasicValue[] Arguments { get; set; } = arguments;

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns></returns>
        public override string? ToString() => $"CallRes{{{Callee}, {TypeExtensions.ToString(Arguments)}}}";

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Callee.Visit(visitor);

                foreach (var arg in Arguments) 
                    arg.Visit(visitor);
            }
        }
    }
}
