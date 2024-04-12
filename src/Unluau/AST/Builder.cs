using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.AST.Statements;

namespace Unluau.AST
{
    /// <summary>
    /// Builds an abstract syntax tree from a program of IL instructions.
    /// </summary>
    public class Builder : IL.Visitor
    {
        /// <summary>
        /// Builds the AST.
        /// </summary>
        /// <param name="program">The program to build an AST from.</param>
        /// <returns>The root.</returns>
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
