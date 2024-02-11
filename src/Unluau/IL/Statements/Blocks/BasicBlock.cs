﻿using Unluau.IL.Statements.Instructions;
using Unluau.IL.Values;

namespace Unluau.IL.Statements.Blocks
{
    /// <summary>
    /// Represents a basic block in the program.
    /// </summary>
    /// <param name="context">Provides context about the block.</param>
    /// <param name="statements">A list of statements.</param>
    public class BasicBlock(Context context, List<Statement> statements) : Statement(context)
    {
        /// <summary>
        /// The statements within the block.
        /// </summary>
        public List<Statement> Statements { get; set; } = statements;

        /// <summary>
        /// Creates a new <see cref="BasicBlock"/>.
        /// </summary>
        /// <param name="context">Context about the block.</param>
        public BasicBlock(Context context) : this(context, [])
        {

        }

        /// <summary>
        /// Visits the children of the current block.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void VisitChildren(Visitor visitor)
        {
            foreach (var statement in Statements)
                statement.Visit(visitor);
        }

        /// <summary>
        /// Recursive visitor method.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                VisitChildren(visitor);
            }
        }
    }
}