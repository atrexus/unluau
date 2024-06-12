using Unluau.Decompile.IL.Values;

namespace Unluau.Decompile.IL.Instructions
{
    /// <summary>
    /// Performs a call operation on a basic value.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="call">The object call.</param>
    /// <param name="slots">The slots the call will fill.</param>
    public class Call(Context context, CallResult call, Slot[] slots) : Instruction(context)
    {
        /// <summary>
        /// The value to perform that call operation on.
        /// </summary>
        public CallResult CallResult { get; set; } = call;

        /// <summary>
        /// The register slots this call is supposed to fill.
        /// </summary>
        public Slot[] Slots { get; private set; } = slots;

        /// <summary>
        /// The recursive visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                CallResult.Visit(visitor);
            }
        }
    }
}
