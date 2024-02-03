using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IL.Values;

namespace Unluau.IL.Instructions
{
    /// <summary>
    /// Gets a value from a table using an index.
    /// </summary>
    /// <param name="context">Information about the location.</param>
    /// <param name="slot">The slot to load into.</param>
    /// <param name="indexable">The object that is gettting indexed.</param>
    /// <param name="index">The value to index the object by.</param>
    public class GetIndex(Context context, Slot slot, BasicValue indexable, BasicValue index) : Instruction(context)
    {
        /// <summary>
        /// The register slot to load the result of the index.
        /// </summary>
        public Slot Slot { get; private set; } = slot;

        /// <summary>
        /// The indexable object (usually a table or array).
        /// </summary>
        public BasicValue Indexable { get; private set; } = indexable;

        /// <summary>
        /// The value to index the object by.
        /// </summary>
        public BasicValue Index { get; private set; } = index;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Indexable.Visit(visitor);
                Index.Visit(visitor);
            }
        }
    }

    /// <summary>
    /// Gets a value from a table using an index and passes a pointer to itself to the proceeding call.
    /// </summary>
    /// <param name="context">Information about the location.</param>
    /// <param name="slot">The slot to load the </param>
    /// <param name="indexable">The object that is gettting indexed.</param>
    /// <param name="index">The value to index the object by.</param>
    public class GetIndexSelf(Context context, Slot slot, BasicValue indexable, BasicValue index) : GetIndex(context, slot, indexable, index)
    {
        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Indexable.Visit(visitor);
                Index.Visit(visitor);
            }
        }
    }
}
