using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IL.Instructions;

namespace Unluau.IL
{
    public class Visitor
    {
        public virtual bool Visit(Node node) => true;

        public virtual bool Visit(Instruction node) => Visit(node as Node);
    }
}
