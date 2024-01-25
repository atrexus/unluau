using Unluau.IL.Values;

namespace Unluau.IL.Instructions
{
    /// <summary>
    /// Performs a call operation on a basic value.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="callee">The value to call.</param>
    /// <param name="arguments">The list of arguments.</param>
    /// <param name="results">Number of values loaded on stack.</param>
    public class Call(Context context, BasicValue callee, BasicValue[] arguments, int? results) : Instruction(context)
    {
        /// <summary>
        /// The value to perform that call operation on.
        /// </summary>
        public BasicValue Callee { get; private set; } = callee;

        /// <summary>
        /// The arguments of the call operation.
        /// </summary>
        public BasicValue[] Arguments { get; private set; } = arguments;

        /// <summary>
        /// The number of return values of the call.
        /// </summary>
        public int? Results { get; private set; } = results;

        /// <summary>
        /// The recursive visitor.
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
