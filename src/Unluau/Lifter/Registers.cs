// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unluau.Declaration;

namespace Unluau
{
    public class Registers
    {
        public int Count { get; private set; } = 0;
        public int Top { get; private set; }
        public bool ReassignOccupiedRegisters { get; set; } = false;

        public IDictionary<int, Declaration> Declarations { get; private set; }
        private IDictionary<int, Expression> expressions;
        private Namer namer;
        private DecompilerOptions options;

        public Registers(Function function, IDictionary<int, Declaration> declarations, IDictionary<int, Expression> expressions, DecompilerOptions options)
        {
            Count = function.MaxStackSize;
            
            this.Declarations = declarations;
            this.expressions = expressions;
            this.options = options;

            namer = new Namer(this);
        }

        public Registers(Function function, DecompilerOptions options)
            : this(function, new Dictionary<int, Declaration>(), new Dictionary<int, Expression>(), options)
        { }

        public Registers(Registers registers)
        {
            Count = registers.Count;

            Declarations = new Dictionary<int,Declaration>(registers.Declarations);
            expressions = new Dictionary<int, Expression>(registers.expressions);
            options = registers.options;

            namer = new Namer(this);
        }

        public void LoadRegister(int register, Expression expression, Block block, int pc, DeclarationType type = DeclarationType.Local)
        {
            LocalExpression local = LoadTempRegister(register, expression, block, type);

            var assignment = new LocalAssignment(local);

            block.AddStatement(assignment, pc);
        }

        // Literally just loads a register but doesn't create a local assignment for it. Mainly used for GETVARARGS
        public LocalExpression LoadTempRegister(int register, Expression expression, Block block, DeclarationType type)
        {
            Declaration declaration = namer.CreateDeclaration(register, expression, block, type, !options.VariableNameGuessing);

            LocalExpression local = new LocalExpression(expression, declaration);

            SetDeclaration(register, declaration);
            SetExpression(register, local);

            Top = register;

            return local;
        }

        public void FreeRegisters(Block block)
        {
            int localsCount = 0;
            int upvaluesCount = 0;
            int closuresCount = 0;

            // Eliminate all dead local assignments
            // TODO: Fix this horrendous code 
            for (int i = 0; i < block.Statements.Count; i++)
            {
                if (block.Statements[i] is LocalAssignment)
                {
                    LocalAssignment assignment = block.Statements[i] as LocalAssignment;

                    if (assignment.TryGetVariable(out LocalExpression variable))
                    {
                        if (variable.Declaration.Referenced == 1)
                        {
                            block.Statements.RemoveAt(i);
                            i--;
                        }
                        else if (Declarations.ContainsKey(variable.Declaration.Register))
                        {
                            // Update the ID of the variable
                            switch (variable.Declaration.Type)
                            {
                                case DeclarationType.Local:
                                    Declarations[variable.Declaration.Register].Id = localsCount++;
                                    break;
                                case DeclarationType.Upvalue:
                                    Declarations[variable.Declaration.Register].Id = upvaluesCount++;
                                    break;
                                case DeclarationType.Closure:
                                    Declarations[variable.Declaration.Register].Id = closuresCount++;
                                    break;
                            }
                        }
                    }
                    else if (assignment.TryGetVariables(out ExpressionList expressions))
                    {
                        foreach (Expression expression in expressions.Expressions)
                        {
                            if (expression is LocalExpression local && Declarations.ContainsKey(local.Declaration.Register))
                            {
                                // Update the ID of the variable
                                switch (local.Declaration.Type)
                                {
                                    case DeclarationType.Local:
                                        Declarations[local.Declaration.Register].Id = localsCount++;
                                        break;
                                    case DeclarationType.Upvalue:
                                        Declarations[local.Declaration.Register].Id = upvaluesCount++;
                                        break;
                                    case DeclarationType.Closure:
                                        Declarations[local.Declaration.Register].Id = closuresCount++;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            namer.PurifyVariableNames();

            Declarations.Clear();
            expressions.Clear();
        }

        public void FreeRegister(int register, Block block)
        {
            Declaration declaration;

            if ((declaration = GetDeclaration(register, false)) != null)
            {
                /*if (declaration.Referenced == 1)
                    block.Statements.RemoveAt(declaration.Location);*/
                // Remove declaration and expression
                Declarations.Remove(register);
                expressions.Remove(register);
            }

            /*            // Remove declaration and expression
                        declarations.Remove(register);
                        expressions.Remove(register);*/
        }

        public void MoveRegister(int fromRegister, int toRegister)
        {
            Declaration declaration = GetDeclaration(fromRegister);

            SetDeclaration(toRegister, declaration);
            SetExpression(toRegister, expressions[fromRegister]);
        }

        public IList<Declaration> GetDeclarations()
        {
            return Declarations.Values.ToList();
        }

        public Declaration GetDeclaration(int register, bool reference = true)
        {
            if (Declarations.ContainsKey(register))
            {
                var declaration = Declarations[register];

                if (reference)
                    declaration.Referenced++;

                SetDeclaration(register, declaration);

                return declaration;
            }

            return null;
        }

        public bool IsEmpty(int register) => !(Declarations.ContainsKey(register) && expressions.ContainsKey(register));

        public void SetDeclaration(int register, Declaration declaration)
        {
            if (Declarations.ContainsKey(register))
            {
                Declarations.Remove(register);
            }
            
            Declarations.Add(register, declaration);
        }

        public Expression GetExpression(int register, bool reference = true)
        {
            Declaration declaration = GetDeclaration(register, reference);
            
            if (expressions.ContainsKey(register))
            {
                if (declaration != null)
                    ((LocalExpression)expressions[register]).Declaration = declaration;
                return expressions[register];
            }

            return null;
        }

        public Expression GetExpressionValue(int register)
        {
            return ((LocalExpression)GetExpression(register, false)).Expression;
        }

        public Expression GetRefExpressionValue(int register)
        {
            return ((LocalExpression)GetExpression(register)).Expression;
        }

        public void SetExpression(int register, Expression expression)
        {
            if (expressions.ContainsKey(register))
                expressions[register] = expression;
            else
                expressions.Add(register, expression);
        }
    }
}
