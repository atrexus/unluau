using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.AST.Statements;

namespace Unluau.AST
{
    public class Builder : IL.Visitor
    {
        public static Block Build(IL.Program program)
        {
            program.Visit(new Builder());
        }

        public Block Root { get; set; }

        private Builder() 
        {
            
        }
    }
}
