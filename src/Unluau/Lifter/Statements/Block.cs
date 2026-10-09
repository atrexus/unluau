// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public class Block : Statement
    {
        public List<Statement> Statements { get; }
        public IDictionary<int, Statement> PcMap { get; }

        public Block(List<Statement> statements)
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
        }

        public void AddStatement(Expression statement, int pc) => AddStatement(new StatementExpression(statement), pc);
    }
}
