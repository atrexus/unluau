// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Diagnostics.CodeAnalysis;
using Unluau.Logic;
using Unluau.Logic.Expressions;
using Unluau.Logic.Statements;

namespace Unluau
{
    public class Registers
    {
        public int Top { get; private set; }

        public IDictionary<int, Declaration> Declarations { get; }
        private readonly IDictionary<int, LocalExpression> _expressions;
        private readonly Namer _namer;
        private readonly DecompilerOptions _options;

        public Registers(IDictionary<int, Declaration> declarations, IDictionary<int, LocalExpression> expressions, DecompilerOptions options)
        {
            this.Declarations = declarations;
            this._expressions = expressions;
            this._options = options;

            _namer = new Namer(this);
        }

        public Registers(Registers registers)
        {
            Declarations = new Dictionary<int, Declaration>(registers.Declarations);
            _expressions = new Dictionary<int, LocalExpression>(registers._expressions);
            _options = registers._options;

            _namer = new Namer(this);
        }

        public void LoadRegister(int register, Expression expression, Block block, int pc, Declaration.DeclarationType type = Declaration.DeclarationType.Local)
        {
            LocalExpression local = LoadTempRegister(register, expression, block, type);

            var assignment = new LocalAssignment(local);

            block.AddStatement(assignment, pc);
        }

        // Literally just loads a register but doesn't create a local assignment for it. Mainly used for GETVARARGS
        public LocalExpression LoadTempRegister(int register, Expression expression, Block block, Declaration.DeclarationType type)
        {
            Declaration declaration = _namer.CreateDeclaration(register, expression, block, type, !_options.VariableNameGuessing);

            LocalExpression local = new LocalExpression(expression, declaration);

            Declarations[register] = declaration;
            _expressions[register] = local;

            Top = register;

            return local;
        }
        
        public void FreeRegisters(Block block)
        {
            int localsCount = 0;
            int upvaluesCount = 0;
            int closuresCount = 0;

            // Eliminate all dead local assignments
            var removedStatements = new List<Statement>();
            foreach (var statement in block.Statements)
            {
                if (statement is not LocalAssignment localAssignment)
                    continue;

                if (localAssignment.TryGetVariable(out LocalExpression? localExpression))
                {
                    if (localExpression.Declaration.Referenced == 1)
                    {
                        removedStatements.Add(localAssignment);
                        continue;
                    }

                    if (!Declarations.TryGetValue(localExpression.Declaration.Register, out var declaration))
                        continue;

                    declaration.Id = localExpression.Declaration.Type switch
                    {
                        Declaration.DeclarationType.Local => localsCount++,
                        Declaration.DeclarationType.Upvalue => upvaluesCount++,
                        Declaration.DeclarationType.Closure => closuresCount++,
                        _ => declaration.Id
                    };
                    continue;
                }

                if (!localAssignment.TryGetVariables(out ExpressionList? expressionList))
                    continue;

                foreach (Expression expression in expressionList.Expressions)
                {
                    // VERIFY: This block is otherwise identical to the one above but does not cull statements from
                    // the list with reference counting. What's the consequence of this?

                    if (expression is not LocalExpression local)
                        continue;

                    if (!Declarations.TryGetValue(local.Declaration.Register, out var declaration))
                        continue;

                    declaration.Id = local.Declaration.Type switch
                    {
                        Declaration.DeclarationType.Local => localsCount++,
                        Declaration.DeclarationType.Upvalue => upvaluesCount++,
                        Declaration.DeclarationType.Closure => closuresCount++,
                        _ => declaration.Id
                    };
                }
            }

            block.Statements.RemoveAll(removedStatements.Contains);

            _namer.PurifyVariableNames();

            Declarations.Clear();
            _expressions.Clear();
        }

        public void FreeRegister(int register)
        {
            Declaration? declaration = GetDeclaration(register, false);

            if (declaration is null)
                return;
            
            Declarations.Remove(register);
            _expressions.Remove(register);
        }

        public IList<Declaration> GetDeclarations()
        {
            return Declarations.Values.ToList();
        }

        [Obsolete("Prefer TryGetDeclaration")]
        public Declaration? GetDeclaration(int register, bool reference = true)
        {
            if (!Declarations.TryGetValue(register, out var declaration))
                return null;

            if (reference)
                declaration.Referenced++;

            return declaration;
        }

        public bool TryGetDeclaration(int register, [NotNullWhen(true)] out Declaration? declaration, bool reference = true)
        {
            if (!Declarations.TryGetValue(register, out declaration))
                return false;

            if (reference)
                declaration.Referenced++;

            return true;
        }

        public bool IsEmpty(int register) => !(Declarations.ContainsKey(register) && _expressions.ContainsKey(register));

        [Obsolete("Prefer TryGetExpression")]
        public LocalExpression? GetExpression(int register, bool reference = true)
        {
            Declaration? declaration = GetDeclaration(register, reference);

            if (!_expressions.TryGetValue(register, out var localExpression))
                return null;

            if (declaration != null)
                localExpression.Declaration = declaration;
            
            return _expressions[register];
        }

        public bool TryGetExpression(int register, [NotNullWhen(true)] out LocalExpression? localExpression, bool reference = true)
        {
            if (!_expressions.TryGetValue(register, out localExpression))
                return false;

            if (TryGetDeclaration(register, out var declaration, reference))
                localExpression.Declaration = declaration;
            
            localExpression = _expressions[register];
            return true;
        }
        
        [Obsolete("Switch to inlining this")]
        public Expression? GetRefExpressionValue(int register)
        {
            return GetExpression(register)?.Expression;
        }
    }
}
