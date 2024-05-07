using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a get import instruction.
    /// </summary>
    public class GetImport(ulong value) : InstructionAD(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
