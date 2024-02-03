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

        /// <summary>
        /// Resolves a list of values in the IL.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The resolved values.</returns>
        private BasicValue[] ResolveValueList(BasicValue[] values)
        {
            var resolved = new BasicValue[values.Length];

            for (int i  = 0; i < values.Length; ++i)
                resolved[i] = ResolveValue(values[i]);

            return resolved;
        }

        /// <summary>
        /// Resolves a basic value in the IL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resolved value.</returns>
        private BasicValue ResolveValue(BasicValue value)
        {
            if (value is Reference reference)
                return ResolveReference(reference);

            return value;
        }

        /// <summary>
        /// Resolves a reference to a register slot.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The resolved value.</returns>
        private BasicValue ResolveReference(Reference reference)
        {
            // If we only have one reference to a slot, then we can just replace this reference
            // with its BasicValue. 
            if (reference.Slot.References == 1)
            {
                if (_lastBlock is not null)
                {
                    var instructions = new List<Instruction>(_lastBlock.Instructions);
                    
                    var match = instructions.Find(match => match.Context == reference.Slot.Value.Context);

                    if (match != null) 
                        instructions.Remove(match);

                    _lastBlock.Instructions = instructions.ToArray();
                }
               
                return reference.Slot.Value;
            }
                
            return reference;
        }
    }
}
