using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// <param name="index">The indexing operation.</param>
    public class GetIndex(Context context, Slot slot, Values.Index index) : Instruction(context)
    {
        /// <summary>
        /// The register slot to load the result of the index.
        /// </summary>
        public Slot Slot { get; set; } = slot;

        /// <summary>
        /// The indexing operation (contains value and key).
        /// </summary>
        public Values.Index Index { get; set; } = index;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Index.Visit(visitor);
            }
        }
    }

    /// <summary>
    /// Gets a value from a table using an index and passes a pointer to itself to the proceeding call.
    /// </summary>
    public class GetIndexSelf : GetIndex
    {
        /// <summary>
        /// Creates a new instance of <see cref="GetIndexSelf"/>.
        /// </summary>
        /// <param name="context">Information about the location.</param>
        /// <param name="slot">The slot to load the </param>
        /// <param name="index">The indexing operation.</param>
        public GetIndexSelf(Context context, Slot slot, Values.Index index) : base(context, slot, index)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="GetIndexSelf"/>.
        /// </summary>
        /// <param name="index">The <see cref="GetIndex"/> to create from.</param>
        public GetIndexSelf(GetIndex index) : base(index.Context, index.Slot, index.Index)
        {

        }

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Index.Visit(visitor);
            }
        }
    }
}
