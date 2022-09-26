using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Block : Statement
    {
        public IList<Statement> Statements { get; protected set; }

        public Block(IList<Statement> statements)
            => Statements = statements;

        public Block()
            : this(new List<Statement>())
        { }

        public override void Write(Output output)
        {
            output.Indent();

            WriteSequence(output, Statements);

            output.Unindent();
        }

        public void AddStatement(Statement statement)
        {
            Statements.Add(statement);
        }

        public void AddStatement(Expression statement)
        {
            Statements.Add(new StatementExpression(statement));
        }
    }
}
