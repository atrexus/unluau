using Unluau.IL.Values;

namespace Unluau.IL.Instructions
{
    /// <summary>
    /// Loads a value into a register slot.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="slot">The register slot to use.</param>
    /// <param name="value">The value to load.</param>
    public class LoadValue(Context context, int slot, BasicValue value) : Instruction(context)
    {
        /// <summary>
        /// The register slot to use.
        /// </summary>
        public int Slot { get; private set; } = slot;

        /// <summary>
        /// The value to load into the provided register.
        /// </summary>
        public BasicValue Value { get; private set; } = value;

        /// <summary>
        /// Implements the recusive visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
                Value.Visit(visitor);
        }
    }
}
