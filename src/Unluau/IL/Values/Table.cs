using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// An entry in a table. Contains a key and a value property.
    /// </summary>
    public sealed record TableEntry
    {
        /// <summary>
        /// Information about the entry.
        /// </summary>
        public required Context Context { get; set; }

        /// <summary>
        /// Key to the entry.
        /// </summary>
        public BasicValue? Key { get; set; }

        /// <summary>
        /// Value for the entry.
        /// </summary>
        public required BasicValue Value { get; set; }

        /// <summary>
        /// Returns a string representation of the entry.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() => Key is null ? $"Entry({Value})" : $"Entry({Key}, {Value})";
    }

    /// <summary>
    /// A table within a Lua program.
    /// </summary>
    public class Table : BasicValue
    {
        /// <summary>
        /// A list of key value entries in the table.
        /// </summary>
        public List<TableEntry> Entries { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Table"/>.
        /// </summary>
        /// <param name="context">Information about the table.</param>
        /// <param name="entries">List of entries in the table.</param>
        public Table(Context context, TableEntry[] entries) : base(context) 
        {
            Entries = new(entries);
        }

        /// <summary>
        /// Creates a new instance of <see cref="Table"/>.
        /// </summary>
        /// <param name="context">Information about the table.</param>
        /// <param name="size">The size of the table.</param>
        public Table(Context context, int size) : base(context)
        {
            Entries = new List<TableEntry>(size);
        }

        /// <summary>
        /// Returns a string representation of the table.
        /// </summary>
        /// <returns></returns>
        public override string? ToString() => $"Table{{{TypeExtensions.ToString(Entries.ToArray())}}}";

        /// <summary>
        /// Implements the visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            foreach (var entry in Entries)
            {
                entry.Value.Visit(visitor);
                entry.Key?.Visit(visitor);
            }
        }
    }
}
