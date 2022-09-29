using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Lifter
    {
        private Chunk chunk;
        private DecompilerOptions options;
        private static int upvalueId = 0;

        private enum CaptureType : byte
        {
            Value,
            Reference,
            Upvalue
        }

        public Lifter(Chunk chunk, DecompilerOptions options)
        { 
            this.chunk = chunk;
            this.options = options;
        }

        public OuterBlock LiftProgram()
        {
            Function main = chunk.Functions[chunk.MainIndex];
            Registers registers = CreateRegisters(main);

           return new OuterBlock(LiftBlock(main, registers));
        }

        private Block LiftBlock(Function function, Registers registers, int pcStart = 0, int pcStop = -1)
        {
            if (pcStop == -1)
                pcStop = function.Instructions.Count;

            Block block = new Block();

            for (int pc = pcStart; pc < pcStop; pc++)
            {
                Instruction instruction = function.Instructions[pc];
                OpProperties properties = instruction.GetProperties();

                switch (properties.Code)
                {
                    case OpCode.LOADKX:
                    case OpCode.LOADK:
                    {
                        Constant target = properties.Code == OpCode.LOADK 
                            ? function.Constants[instruction.D] : function.Constants[(int)function.Instructions[++pc].Value];

                        registers.LoadRegister(instruction.A, ConstantToExpression(target), block);
                        break;
                    }
                    case OpCode.LOADN:
                    {
                        registers.LoadRegister(instruction.A, new NumberLiteral(instruction.D), block);
                        break;
                    }
                    case OpCode.GETGLOBAL:
                    {
                        Constant target = function.Constants[(int)function.Instructions[++pc].Value];

                        registers.LoadRegister(instruction.A, GetConstantAsGlobal(target), block);
                        break;
                    }
                    case OpCode.SETGLOBAL:
                    {
                        Constant target = function.Constants[(int)function.Instructions[++pc].Value];

                        block.AddStatement(new Assignment(GetConstantAsGlobal(target), registers.GetExpression(instruction.A)));
                        break;
                    }
                    case OpCode.GETIMPORT:
                    {
                        ImportConstant target = (ImportConstant)function.Constants[instruction.D];

                        if (target.Value.Count > 1)
                        {
                            Expression expression = new NameIndex(new Global(target.Value[0].Value), target.Value[1].Value);

                            if (target.Value.Count > 2)
                                expression = new NameIndex(expression, target.Value[2].Value);

                            registers.LoadRegister(instruction.A, expression, block);
                        } 
                        else
                            registers.LoadRegister(instruction.A, GetConstantAsGlobal(target), block);
                        
                        // Skip next instruction (we used the constant instead of AUX)
                        pc++;
                        break;
                    }
                    case OpCode.GETTABLEKS:
                    {
                        Constant target = function.Constants[(int)function.Instructions[++pc].Value];

                        Expression expression = new NameIndex(registers.GetExpression(instruction.B), ((StringConstant)target).Value);

                        registers.LoadRegister(instruction.A, expression, block);
                        break;
                    }
                    case OpCode.GETTABLEN:
                    {
                        ExpressionIndex index = new ExpressionIndex(registers.GetExpression(instruction.B), new NumberLiteral(instruction.C + 1));

                        registers.LoadRegister(instruction.A, index, block);
                        break;
                    }
                    case OpCode.GETTABLE:
                    {
                        ExpressionIndex expressionIndex = new ExpressionIndex(registers.GetExpression(instruction.B), registers.GetExpression(instruction.C));

                        registers.LoadRegister(instruction.A, expressionIndex, block);
                        break;
                    }
                    case OpCode.CALL:
                    {
                        IList<Expression> arguments = new List<Expression>();

                        Expression callFunction = registers.GetExpression(instruction.A);

                        if (instruction.B > 0)
                        {
                            for (int slot = 1; slot < instruction.B; ++slot)
                                arguments.Add(registers.GetExpression(instruction.A + slot));
                        }

                        FunctionCall call = new FunctionCall(callFunction, arguments);

                        if (instruction.C - 1 == 0)
                            block.AddStatement(call);
                        else
                            registers.LoadRegister(instruction.A, call, block);
                        break;
                    }
                    case OpCode.MOVE:
                    {
                        registers.MoveRegister(instruction.B, instruction.A);
                        break;
                    }
                    case OpCode.LOADNIL:
                    {
                        registers.LoadRegister(instruction.A, new NilLiteral(), block);
                        break;
                    }
                    case OpCode.LOADB:
                    {
                        registers.LoadRegister(instruction.A, new BooleanLiteral(instruction.B == 1), block);
                        break;
                    }
                    case OpCode.ADDK:
                    case OpCode.SUBK:
                    case OpCode.MULK:
                    case OpCode.DIVK:
                    case OpCode.MODK:
                    case OpCode.POWK:
                    {
                        Expression expression = new BinaryExpression(registers.GetExpression(instruction.B), BinaryExpression.GetBinaryOperation(properties.Code), 
                            ConstantToExpression(function.Constants[instruction.C]));

                        registers.LoadRegister(instruction.A, expression, block);
                        break;
                    }
                    case OpCode.ADD:
                    case OpCode.SUB:
                    case OpCode.MUL:
                    case OpCode.DIV:
                    case OpCode.MOD:
                    case OpCode.POW:
                    {
                        Expression expression = new BinaryExpression(registers.GetExpression(instruction.B), BinaryExpression.GetBinaryOperation(properties.Code), 
                            registers.GetExpression(instruction.C));

                        registers.LoadRegister(instruction.A, expression, block);
                        break;
                    }
                    case OpCode.CONCAT:
                    {
                        // Kinda annoying, gotta unravel the whole TING
                        Expression expression = BuildConcat(registers, instruction.B, instruction.C);

                        registers.LoadRegister(instruction.A, expression, block);
                        break;
                    }
                    case OpCode.NOT:
                    case OpCode.LENGTH:
                    case OpCode.MINUS:
                    {
                        registers.LoadRegister(instruction.A, registers.GetExpression(instruction.B), block);
                        break;
                    }
                    case OpCode.SETTABLEKS:
                    {
                        StringConstant target = (StringConstant)function.Constants[(int)function.Instructions[++pc].Value];
                        Expression table = registers.GetExpression(instruction.B), value = ((LocalExpression)table).Expression;

                        if (options.InlineTableDefintions && value is TableLiteral)
                        {
                            TableLiteral tableLiteral = (TableLiteral)value;

                            if (tableLiteral.MaxEntries > tableLiteral.Entries.Count)
                            {
                                tableLiteral.AddEntry(new TableLiteral.Entry(GetConstantAsGlobal(target), registers.GetExpression(instruction.A)));
                                break;
                            }
                        }

                        NameIndex nameIndex = new NameIndex(table, target.Value);

                        block.AddStatement(new Assignment(nameIndex, registers.GetExpression(instruction.A)));
                        break;
                    }
                    case OpCode.SETTABLE:
                    {
                        Expression expression = registers.GetExpressionValue(instruction.C), value = registers.GetExpression(instruction.A);
                        Expression table = registers.GetExpression(instruction.B), tableValue = ((LocalExpression)table).Expression;           

                        if (options.InlineTableDefintions && tableValue is TableLiteral)
                        {
                            TableLiteral tableLiteral = (TableLiteral)tableValue;

                            if (tableLiteral.MaxEntries > tableLiteral.Entries.Count)
                            {
                                tableLiteral.AddEntry(new TableLiteral.Entry(expression, value));
                                break;
                            }
                        }

                        if (expression is NumberLiteral || expression is StringLiteral)
                            expression = new ExpressionIndex(table, expression);

                        block.AddStatement(new Assignment(expression, value));
                        break;
                    }
                    case OpCode.NEWTABLE:
                    {
                        int arraySize = (int)function.Instructions[++pc].Value;
                        int hashSize = instruction.B == 0 ? 0 : (1 << (instruction.B - 1));

                        TableLiteral expression;

                        if (arraySize > 0)
                            expression = new TableLiteral(arraySize, true);
                        else
                            expression = new TableLiteral(hashSize, false);

                        if (options.InlineTableDefintions && hashSize > 0)
                            expression.MaxEntries = hashSize;

                        registers.LoadRegister(instruction.A, expression, block);
                        break;
                    }
                    case OpCode.SETLIST:
                    {
                        TableLiteral tableLiteral = (TableLiteral)registers.GetExpressionValue(instruction.A);

                        for (int slot = instruction.B; slot < instruction.C; slot++)
                            tableLiteral.AddEntry(new TableLiteral.Entry(null, registers.GetExpressionValue(slot)));

                        // Skip next instruction because we didn't use AUX
                        pc++;
                        break;
                    }
                    case OpCode.DUPTABLE:
                    {
                        TableConstant target = (TableConstant)function.Constants[instruction.D];

                        TableLiteral tableLiteral = new TableLiteral(target.Value.Count, false);

                        if (options.InlineTableDefintions)    
                            tableLiteral.MaxEntries = target.Value.Count;
                        

                        registers.LoadRegister(instruction.A, tableLiteral, block);
                        break;
                    }
                    case OpCode.GETVARARGS:
                    {
                        registers.LoadRegister(instruction.A, new Vararg(), block);
                        break;
                    }
                    case OpCode.JUMPIF:
                    case OpCode.JUMPIFNOT:
                    case OpCode.JUMPIFEQ:
                    case OpCode.JUMPIFNOTEQ:
                    case OpCode.JUMPIFNOTLT:
                    case OpCode.JUMPIFLT:
                    case OpCode.JUMPIFNOTLE:
                    case OpCode.JUMPIFLE:
                    case OpCode.JUMPXEQKNIL:
                    case OpCode.JUMPXEQKB:
                    case OpCode.JUMPXEQKN:
                    case OpCode.JUMPXEQKS:
                    {
                        bool auxUsed = false;
                        Expression condition = GetCondition(registers, instruction, function.Instructions[pc + 1], function.Constants, ref auxUsed);

                        Statement statement = new IfElse(condition, LiftBlock(function, registers, auxUsed ? pc + 2 : pc + 1, (pc += instruction.D) + 1));

                        Instruction nextInstruction = function.Instructions[pc];
                        switch (nextInstruction.GetProperties().Code)
                        {
                            case OpCode.JUMP:
                            {
                                IfElse ifElse = statement as IfElse;

                                ifElse.ElseBody = LiftBlock(function, registers, pc + 1, (pc += nextInstruction.D) + 1);
                                Block elseBodyBlock = ifElse.ElseBody as Block; // We know it has to be a block

                                // Little optimization to include elseifs
                                if (elseBodyBlock.Statements.Count == 1 && elseBodyBlock.Statements[0] is IfElse)
                                    ifElse.ElseBody = elseBodyBlock.Statements[0] as IfElse;
                                break;
                            }
                            case OpCode.JUMPBACK:
                            {
                                IfElse ifElse = statement as IfElse;

                                statement = new WhileLoop(ifElse.Condition, ifElse.IfBody);
                                break;
                            }
                        }

                        block.AddStatement(statement);
                        break;
                    }
                    case OpCode.NEWCLOSURE:
                    case OpCode.DUPCLOSURE:
                    {
                        Function newFunction = function.Functions[properties.Code == OpCode.DUPCLOSURE ? (function.Constants[instruction.D] as ClosureConstant).Value : instruction.D];
                        Registers newRegisters = CreateRegisters(newFunction);

                        while (newFunction.Upvalues.Count < newFunction.MaxUpvalues)
                        {
                            Instruction capture = function.Instructions[++pc];

                            if (capture.GetProperties().Code != OpCode.CAPTURE)
                                throw new DecompilerException(Stage.Lifter, "Expected capture instruction following NEWCLOSURE/DUPCLOSURE");

                            LocalExpression expression = null;

                            switch ((CaptureType)capture.A)
                            {
                                case CaptureType.Value:
                                    expression = (LocalExpression)registers.GetExpression(capture.B);
                                    break;
                            }

                            expression.Decleration.Referenced++;

                            if (options.RenameUpvalues)
                            {
                                expression.Decleration.Name = "upval" + ++upvalueId;
                                Decleration.IdCounter--;
                            }
                            
                            newFunction.Upvalues.Add(expression);
                        }

                        registers.LoadRegister(instruction.A, new Closure(newRegisters.GetDeclerations(), newFunction.IsVararg, LiftBlock(newFunction, newRegisters)), block);
                        break;
                    }
                    case OpCode.GETUPVAL:
                    {
                        registers.LoadRegister(instruction.A, function.Upvalues[instruction.B], block);
                        break;
                    }
                }
            }

            registers.FreeRegisters(block);

            return block;
        }

        private Expression BuildConcat(Registers registers, int from, int to)
        {
            Expression left = registers.GetExpression(from);
            Expression right = (from + 1) == to ? registers.GetExpression(to) : BuildConcat(registers, from + 1, to);

            return new BinaryExpression(left, BinaryExpression.BinaryOperation.Concat, right);
        }

        private Expression GetCondition(Registers registers, Instruction instruction, Instruction aux, IList<Constant> constants, ref bool auxUsed)
        {
            auxUsed = false;

            OpCode code = instruction.GetProperties().Code;

            if (code == OpCode.JUMPIF || code == OpCode.JUMPIFNOT)
                return code == OpCode.JUMPIFNOT ? registers.GetExpression(instruction.A)
                            : new UnaryExpression(registers.GetExpression(instruction.A), UnaryExpression.UnaryOperation.Not);

            auxUsed = true;

            BinaryExpression.BinaryOperation operation = BinaryExpression.GetBinaryOperation(code);

            if (operation == BinaryExpression.BinaryOperation.CompareGt || operation == BinaryExpression.BinaryOperation.CompareGe)
                return new BinaryExpression(registers.GetExpression((int)aux.Value), operation, registers.GetExpression(instruction.A));

            Expression right = registers.GetExpression((int)aux.Value);

            if (operation == BinaryExpression.BinaryOperation.CompareEq && code != OpCode.JUMPIFEQ)
            {
                if (code == OpCode.JUMPXEQKN || code == OpCode.JUMPXEQKS)
                    right = ConstantToExpression(constants[(int)aux.Value & 0xffffff]);
                else
                    right = ConstantToExpression(constants[(int)aux.Value]);
            }

            return new BinaryExpression(registers.GetExpression(instruction.A), operation, right);
        }

        private Registers CreateRegisters(Function function)
        {
            IDictionary<int, Decleration> declerations = LoadDeclerations(function);
            IDictionary<int, Expression> expressions = LoadExpressions(declerations);


            return new Registers(function, declerations, expressions);
        }

        private IDictionary<int, Decleration> LoadDeclerations(Function function)
        {
            IDictionary<int, Decleration> declerations = new Dictionary<int, Decleration>();

            if (function.DebugInfo != null)
            {
                // Load the entire debug info into the declerations
                foreach (LocalVariable variable in function.DebugInfo.Locals)
                    declerations[variable.Slot] = new Decleration(variable);
            }
            else
            {
                for (int slot = 0; slot < function.Parameters; slot++)
                    declerations[slot] = new Decleration(slot, "arg" + (slot + 1), 0);
            }

            return declerations;
        }

        private IDictionary<int, Expression> LoadExpressions(IDictionary<int, Decleration> declerations)
        {
            IDictionary<int, Expression> expressions = new Dictionary<int, Expression>();

            foreach (KeyValuePair<int, Decleration> decleration in declerations)
                expressions[decleration.Key] = new LocalExpression(null, decleration.Value);

            return expressions;
        }

        private Expression ConstantToExpression(Constant constant)
        {
            if (constant is StringConstant)
                return new StringLiteral(((StringConstant)constant).Value);

            if (constant is NumberConstant)
                 return new NumberLiteral(((NumberConstant)constant).Value);

            if (constant is BoolConstant)
                return new BooleanLiteral(((BoolConstant)constant).Value);

            if (constant is NilConstant)
                return new NilLiteral();

            if (constant is ImportConstant)
            {
                ImportConstant import = (ImportConstant)constant;
                IList<string> names = new List<string>(import.Value.Count);

                foreach (StringConstant stringConstant in import.Value)
                    names.Add(stringConstant.Value);

                return new Global(names);
            }

            throw new DecompilerException(Stage.Lifter, "unexpected constant passed to 'ConstantToExpression'");
        }

        private Expression GetConstantAsGlobal(Constant constant)
        {
            Expression expression;

            if ((expression = ConstantToExpression(constant)) is StringLiteral)
                return new Global(((StringLiteral)expression).Value);

            if (constant.Type != ConstantType.Import)
                throw new DecompilerException(Stage.Lifter, $"unable to get Import from constant of type \"{constant.Type}\"");

            return expression;
        }
    }
}
