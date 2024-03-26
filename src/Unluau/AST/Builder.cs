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
            var builder = new Builder();
            program.Visit(builder);

            return builder.Root;
        }

        public Block Root { get; set; }

        private Builder() 
        {
            
        }
    }
}
