namespace Unluau.IL.Values
{
    /// <summary>
    /// Represents an index between two separate values in the IL.
    /// </summary>
    /// <param name="indexable">The value to get indexed.</param>
    /// <param name="index">The index key.</param>
    public class Index(Context context, BasicValue indexable, BasicValue key) : BasicValue(context)
    {
        /// <summary>
        /// The value that will get indexed.
        /// </summary>
        public BasicValue Indexable { get; set; } = indexable;

        /// <summary>
        /// The key to use when indexing.
        /// </summary>
        public BasicValue Key { get; set; } = key;  

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>String</returns>
        public override string? ToString() => $"Idx({Indexable}, {Key})";

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Indexable.Visit(visitor);
                Key.Visit(visitor);
            }
        }
    }
}
