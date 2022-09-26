using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Registers
    {
        public int Count { get; private set; }

        private IDictionary<int, Decleration> declerations;
        private IDictionary<int, Expression> expressions;

        public Registers(Function function, IDictionary<int, Decleration> declerations)
        {
            Count = function.MaxStackSize;
            this.declerations = declerations;

            expressions = new Dictionary<int, Expression>(Count);
        }

        public Registers(Function function)
            : this(function, new Dictionary<int, Decleration>())
        { }

        public void LoadRegister(int register, Expression expression, Block block)
        {
            //FreeRegister(register, block);

            Decleration decleration = new Decleration(register, block.Statements.Count);
            LocalExpression local = new LocalExpression(expression, decleration);

            SetDecleration(register, decleration);
            SetExpression(register, local);

            block.AddStatement(new LocalAssignment(local));
        }

        public void FreeRegisters(Block block)
        {
            // Eliminate all dead local assignments
            for (int i = 0; i < block.Statements.Count; i++)
            {
                if (block.Statements[i] is LocalAssignment)
                {
                    LocalAssignment assignment = block.Statements[i] as LocalAssignment;

                    if (assignment.Variable.Decleration.Referenced == 0)
                    {
                        block.Statements.RemoveAt(i);
                        i--;
                    }
                }
            }

            declerations.Clear();
            expressions.Clear();
        }

        public void FreeRegister(int register, Block block)
        {
            Decleration decleration;

            if ((decleration = GetDecleration(register)) != null)
            {
                if (decleration.Referenced < 1)
                    block.Statements.RemoveAt(decleration.Location);
            }
        }

        public void MoveRegister(int fromRegister, int toRegister)
        {
            Decleration decleration = GetDecleration(fromRegister);
            decleration.Referenced++;

            SetDecleration(toRegister, decleration);
            SetExpression(toRegister, expressions[fromRegister]);
        }

        public Decleration GetDecleration(int register)
        {
            if (declerations.ContainsKey(register))
                return declerations[register];
            
            return null;
        }

        public void SetDecleration(int register, Decleration decleration)
        {
            if (declerations.ContainsKey(register))
                declerations[register] = decleration;
            else
                declerations.Add(register, decleration);
        }

        public Expression GetExpression(int register)
        {
            Decleration decleration = GetDecleration(register);

            if (expressions.ContainsKey(register))
            {
                if (decleration != null)
                    ((LocalExpression)expressions[register]).Decleration = decleration;
                return expressions[register];
            }

            return null;
        }

        public Expression GetExpressionValue(int register)
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
