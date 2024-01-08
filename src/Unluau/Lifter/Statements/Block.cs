// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

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
        public IDictionary<int, Statement> PcMap { get; protected set; }

        private IDictionary<int, Expression> _definitions = new Dictionary<int, Expression>();

        public Block(IList<Statement> statements)
        {
            Statements = statements;
            PcMap = new Dictionary<int, Statement>();   
        }

        public Block()
            : this(new List<Statement>())
        { }

        public override void Write(Output output)
        {
            output.Indent();

            WriteSequence(output, Statements);

            output.Unindent();
        }

        public void AddStatement(Statement statement, int pc)
        {
            Statements.Add(statement);
            PcMap.Add(pc, statement);

            Expression? value = null;

            if (statement is Assignment assignment)
                value = assignment.Value;
            else if (statement is LocalAssignment local)
                value = local.Value;

            if (value is not null && value is Closure closure)
            {

            }
        }

        public void UpdateMethod(NameIndex nameIndex)
        {
            //if (_metatableMethods.Contains(nameIndex))
        }

        public void AddStatement(Expression statement, int pc) => AddStatement(new StatementExpression(statement), pc);
    }
}
