using System.Xml.Linq;
using Unluau.IL.Blocks;
using Unluau.IL.Instructions;
using Unluau.IL.Values;

namespace Unluau.IL.Visitors
{
    /// <summary>
    /// Visits the IL and resolves references if possible.
    /// </summary>
    public class ValueVisitor : Visitor
    {
        private BasicBlock? _lastBlock;

        public override bool Visit(Closure node)
        {
            // First we need to resolve all of our references.
            node.VisitChildren(this);

            // Now we need to remove all of the dead instructions.
            node.VisitChildren(this);

            return false;
        }
        public override bool Visit(BasicBlock node)
        {
            _lastBlock = node;

            return true;
        }

        public override bool Visit(Call node)
        {
            node.Callee = ResolveValue(node.Callee);
            node.Arguments = ResolveValueList(node.Arguments);

            return true;
        }

        public override bool Visit(GetIndexSelf node)
        {
            if (!TryDelete(node, node.Slot))
                node.Index = ResolveIndex(node.Index);

            return true;
        }

        public override bool Visit(LoadValue node)
        {
            TryDelete(node, node.Slot);

            return true;
        }

        /// <summary>
        /// Resolves a list of values in the IL.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The resolved values.</returns>
        private static BasicValue[] ResolveValueList(BasicValue[] values)
        {
            var resolved = new BasicValue[values.Length];

            for (int i = 0; i < values.Length; ++i)
                resolved[i] = ResolveValue(values[i]);

            return resolved;
        }

        /// <summary>
        /// Resolves a basic value in the IL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resolved value.</returns>
        private static BasicValue ResolveValue(BasicValue value)
        {
            if (value is Reference reference)
                return ResolveReference(reference);
            if (value is Values.Index index)
                return ResolveIndex(index);

            return value;
        }

        /// <summary>
        /// Resolves a reference to a register slot.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The resolved value.</returns>
        private static BasicValue ResolveReference(Reference reference)
        {
            // If we only have one reference to a slot, then we can just replace this reference
            // with its BasicValue. 
            if (reference.Slot.References == 1)
            {
                // We set the number of references to a negative value so that the load instruction 
                // for this slot can be removed.
                reference.Slot.References = -1;

                return reference.Slot.Value;
            }

            return reference;
        }

        /// <summary>
        /// Resolves an index operation.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The resolved index.</returns>
        private static Values.Index ResolveIndex(Values.Index index)
        {
            index.Indexable = ResolveValue(index.Indexable);
            index.Key = ResolveValue(index.Key);

            return index;
        }

        /// <summary>
        /// Tries to delete an instruction from the instructions list if its "dead".
        /// </summary>
        /// <param name="node">The instruction</param>
        /// <param name="slot">Its slot.</param>
        /// <returns>True if deleted.</returns>
        private bool TryDelete(Instruction node, Slot slot)
        {
            if (slot.References == -1 && _lastBlock != null)
            {
                _lastBlock.Instructions.Remove(node);
                return true;
            }

            return false;
        }
    }
}
