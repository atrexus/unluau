using Unluau.IL.Values;

namespace Unluau.IL
{
    /// <summary>
    /// Contains information on a frame of register slots.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="Stack"/> and copies the provided slots.
    /// </remarks>
    /// <param name="slots">The slots to copy.</param>
    public class Stack(Dictionary<int, Slot> slots)
    {
        private readonly Dictionary<int, Slot> _slots = slots;

        /// <summary>
        /// Gets the register slot on the top of the stack.
        /// </summary>
        public Slot Top => _slots[_slots.Keys.ToHashSet().Max()];

        /// <summary>
        /// Creates a new <see cref="Stack"/>.
        /// </summary>
        public Stack() : this([])
        {
        }

        /// <summary>
        /// Clones the current <see cref="Stack"/>.
        /// </summary>
        /// <returns>The cloned stack.</returns>
        public Stack Clone() => new(_slots);

        /// <summary>
        /// Sets the slot to the specified value.
        /// </summary>
        /// <param name="id">The slot number.</param>
        /// <param name="basicValue">The value to set.</param>
        public Slot Set(byte id, BasicValue basicValue)
        {
            if (_slots.ContainsKey(id))
                Free(id);

            var value = new Slot() 
            { 
                Id = id, 
                Value = basicValue 
            };

            _slots.Add(id, value);

            return value;
        }

        /// <summary>
        /// Updates the slot with the specified value.
        /// </summary>
        /// <param name="id">The slot number.</param>
        /// <param name="basicValue">The value to set.</param>
        /// <returns>The new slot.</returns>
        public Slot Update(byte id, BasicValue basicValue)
        {
            if (!_slots.ContainsKey(id))
                Set(id, basicValue);

            var slot = _slots[id];

            slot.Value = basicValue;

            return slot;
        }

        /// <summary>
        /// Gets a slot with the specific slot number.
        /// </summary>
        /// <param name="id">The slot number.</param>
        /// <returns>The slot.</returns>
        /// <exception cref="InvalidOperationException">If the slot doesn't have a value.</exception>
        public Slot? Get(byte id)
        {
            if (_slots.TryGetValue(id, out Slot? value))
            {
                value.References++;
                return value;
            }

            return null;
        }

        /// <summary>
        /// Frees the slot with the provided slot number.
        /// </summary>
        /// <param name="id">The slot number.</param>
        public void Free(byte id) => _slots.Remove(id);

        /// <summary>
        /// Frees the current stack frame (baseSlot..Top).
        /// </summary>
        /// <param name="baseSlot">The base register slot.</param>
        public void FreeFrame(byte baseSlot)
        {
            for (byte slot = baseSlot; slot < Top.Id; slot++)
                _slots.Remove(slot);
        }

        /// <summary>
        /// Tries to get a slot if it has been initialized.
        /// </summary>
        /// <param name="id">The slot number.</param>
        /// <param name="value">The slot.</param>
        /// <returns>True if successful.</returns>
        public bool TryGet(byte id, out Slot? value) => _slots.TryGetValue(id, out value);

        /// <summary>
        /// Gets a slot with the specific slot number.
        /// </summary>
        /// <param name="v">The slot number.</param>
        /// <returns>The slot.</returns>
        internal Slot? Get(int v) => Get((byte)v);

        /// <summary>
        /// Sets the slot to the specified value.
        /// </summary>
        /// <param name="v">The slot number.</param>
        /// <param name="basicValue">The value to set.</param>
        internal Slot Set(int v, BasicValue basicValue) => Set((byte)v, basicValue);
    }

    /// <summary>
    /// Represents a register slot in the IL.
    /// </summary>
    public class Slot
    {
        /// <summary>
        /// The unique slot number.
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// The number of times this slot's value was requested.
        /// </summary>
        public int References { get; set; }

        /// <summary>
        /// The value assigned to the register.
        /// </summary>
        public required BasicValue Value { get; set; }

        /// <summary>
        /// Returns a string representation of the slot number.
        /// </summary>
        /// <returns>Slot number as string.</returns>
        public override string ToString() => $"R({Id})";
    }
}
