// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Serilog;
using Unluau.Format.Models;
using Unluau.Logic;
using Unluau.Logic.Expressions;
using Unluau.Logic.Statements;
using Unluau.Utilities;

namespace Unluau
{
    public class Lifter
    {
        private readonly Chunk _chunk;
        private readonly DecompilerOptions _options;

        private enum CaptureType : byte
        {
            Value,
            Reference,
            Upvalue
        }

        public Lifter(Chunk chunk, DecompilerOptions options)
        {
            this._chunk = chunk;
            this._options = options;
        }

        public OuterBlock LiftProgram()
        {
            Function main = _chunk.Functions[_chunk.MainIndex];
            Registers registers = main.CreateRegisters(_options);

            var block = new OuterBlock(LiftBlock(main, registers));

            // Note: these values doesn't get cleared after each test in Unluau.Test so we have to do it
            // manually here.
            Declaration.ResetCounters();

            return block;
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
                            ? function.Constants[instruction.D] : function.GetConstant(++pc);

                        registers.LoadRegister(instruction.A, target.ToExpression(), block, pc);
                        break;
                    }
                    case OpCode.LOADN:
                    {
                        registers.LoadRegister(instruction.A, new NumberLiteral(instruction.D), block, pc);
                        break;
                    }
                    case OpCode.GETGLOBAL:
                    {
                        Constant target = function.GetConstant(++pc);

                        registers.LoadRegister(instruction.A, target.ToExpressionGlobal(), block, pc);
                        break;
                    }
                    case OpCode.SETGLOBAL:
                    {
                        Constant target = function.GetConstant(++pc);

                        block.AddStatement(new Assignment(target.ToExpressionGlobal(), registers.GetExpressionOrThrow(instruction.A)), pc);
                        break;
                    }
                    case OpCode.GETIMPORT:
                    {
                        var target = function.GetConstantOrThrow<ImportConstant>(instruction.D);

                        if (target.Value.Count > 1)
                        {
                            Expression expression = new NameIndex(new Global(target.Value[0].Value), target.Value[1].Value);

                            if (target.Value.Count > 2)
                                expression = new NameIndex(expression, target.Value[2].Value);

                            registers.LoadRegister(instruction.A, expression, block, pc);
                        }
                        else
                            registers.LoadRegister(instruction.A, target.ToExpressionGlobal(), block, pc);

                        // Skip next instruction (we used the constant instead of AUX)
                        pc++;
                        break;
                    }
                    case OpCode.NAMECALL:
                    case OpCode.GETTABLEKS:
                    case OpCode.GETUDATAKS:
                    {
                        var aux = function.Instructions[++pc];
                        var targetValue = function.GetConstantOrThrow<StringConstant>((int)aux.Value).Value;
                        bool isNamecall = properties.Code == OpCode.NAMECALL;
                        Expression nameExpression = registers.GetExpressionOrThrow(instruction.B);

                        Expression expression = new NameIndex(nameExpression, targetValue, isNamecall);

                        // This is a namecall and the function name is format we need to account for string interpolation
                        if (isNamecall && targetValue == "format")
                        {
                            // Extract the name index from the expression
                            NameIndex? nameIndex = expression as NameIndex;

                            if (_options.PerferStringInterpolation)
                            {
                                // Safely extract the format string
                                var localExpr = nameIndex?.Expression as LocalExpression;
                                if (localExpr?.Expression is StringLiteral stringLiteral)
                                {
                                    string format = stringLiteral.Value;
                                    expression = new InterpolatedStringLiteral(format, new List<Expression>());
                                }
                            }
                            else
                            {
                                // Otherwise add an expression group around the string literal
                                if (nameIndex != null)
                                {
                                    nameIndex.Expression = new ExpressionGroup(nameIndex.Expression);
                                    expression = nameIndex;
                                }
                            }
                        }

                        registers.LoadRegister(instruction.A, expression, block, pc);
                        break;
                    }
                    case OpCode.GETTABLEN:
                    {
                        var tableExpression = registers.GetExpressionOrThrow(instruction.B);
                        var numberLiteral = new NumberLiteral(instruction.C + 1);
                        ExpressionIndex index = new ExpressionIndex(tableExpression, numberLiteral);

                        registers.LoadRegister(instruction.A, index, block, pc);
                        break;
                    }
                    case OpCode.GETTABLE:
                    {
                        var tableExpression = registers.GetExpressionOrThrow(instruction.B);
                        var indexExpression = registers.GetExpressionOrThrow(instruction.C);
                        ExpressionIndex expressionIndex = new ExpressionIndex(tableExpression, indexExpression);

                        registers.LoadRegister(instruction.A, expressionIndex, block, pc);
                        break;
                    }
                    case OpCode.FASTCALL:
                    case OpCode.CALL:
                    {
                        Builtin? builtin = null;

                        if (properties.Code == OpCode.FASTCALL)
                        {
                            builtin = Builtin.FromId(instruction.A);

                            instruction = function.Instructions[pc += instruction.C + 1];
                        }

                        IList<Expression> arguments = new List<Expression>();

                        Expression callFunction = builtin is null ? registers.GetExpressionOrThrow(instruction.A) : builtin.Expression;

                        int numArgs = instruction.B > 0 ? instruction.B : (registers.Top - instruction.A) + 1;

                        var isSelfCall = callFunction.GetValue() is NameIndex { IsSelf: true };
                        for (int slot = 1 + (isSelfCall ? 1 : 0); slot < numArgs; ++slot)
                        {
                            int register = instruction.A + slot;
                            if (!registers.TryGetExpression(register, out var expression))
                                continue;

                            arguments.Add(expression);
                            registers.FreeRegister(register);
                        }

                        // Free the function register if we used it
                        if (builtin is null)
                            registers.FreeRegister(instruction.A);

                        Expression call = new FunctionCall(callFunction, arguments);

                        // Note: we do this for string interpolation to work
                        var callFunctionValue = callFunction.GetValue();
                        if (callFunctionValue is InterpolatedStringLiteral literal)
                        {
                            literal.Arguments = arguments;
                            call = literal;
                        }

                        if (instruction.C - 1 == 0)
                            block.AddStatement(call, pc);
                        else
                            registers.LoadRegister(instruction.A, call, block, pc);
                        break;
                    }
                    case OpCode.MOVE:
                    {
                        var fromExpression = registers.GetExpression(instruction.B);

                        // If our target register is empty, then load into that register
                        if (registers.IsEmpty(instruction.A))
                            registers.LoadRegister(instruction.A, fromExpression, block, pc);
                        else
                        {
                            // Now create the reassignment
                            var assignment = new Assignment(
                                registers.GetExpressionOrThrow(instruction.A), fromExpression);
                            block.AddStatement(assignment, pc);
                        }
                        break;
                    }
                    case OpCode.LOADNIL:
                    {
                        registers.LoadRegister(instruction.A, new NilLiteral(), block, pc);
                        break;
                    }
                    case OpCode.LOADB:
                    {
                        registers.LoadRegister(instruction.A, new BooleanLiteral(instruction.B == 1), block, pc);
                        break;
                    }

                    // Uses register for right hand expression
                    case OpCode.ADD:
                    case OpCode.SUB:
                    case OpCode.MUL:
                    case OpCode.DIV:
                    case OpCode.IDIV:
                    case OpCode.MOD:
                    case OpCode.POW:
                    case OpCode.AND:
                    case OpCode.OR:
                    // Uses constant for right hand expression
                    case OpCode.ADDK:
                    case OpCode.SUBK:
                    case OpCode.MULK:
                    case OpCode.DIVK:
                    case OpCode.IDIVK:
                    case OpCode.MODK:
                    case OpCode.POWK:
                    case OpCode.ANDK:
                    case OpCode.ORK:
                    // Uses constants for left-hand expression
                    case OpCode.SUBRK:
                    case OpCode.DIVRK:
                    {
                        Expression right;
                        Expression left;

                        switch (properties.Code)
                        {
                            case OpCode.SUBRK:
                            case OpCode.DIVRK:
                                right = registers.GetExpressionOrThrow(instruction.C);
                                left = function.Constants[instruction.B].ToExpression();
                                break;
                            case OpCode.ADD:
                            case OpCode.SUB:
                            case OpCode.MUL:
                            case OpCode.DIV:
                            case OpCode.IDIV:
                            case OpCode.MOD:
                            case OpCode.POW:
                            case OpCode.AND:
                            case OpCode.OR:
                                right = registers.GetExpressionOrThrow(instruction.C);
                                left = registers.GetExpressionOrThrow(instruction.B);
                                break;
                            case OpCode.ADDK:
                            case OpCode.SUBK:
                            case OpCode.MULK:
                            case OpCode.DIVK:
                            case OpCode.IDIVK:
                            case OpCode.MODK:
                            case OpCode.POWK:
                            case OpCode.ANDK:
                            case OpCode.ORK:
                                right = function.Constants[instruction.C].ToExpression();
                                left = registers.GetExpressionOrThrow(instruction.B);
                                break;
                            default:
                                throw new DecompilerException(Stage.Lifter, $"Unrecognized opcode: {properties.Code}");
                        }

                        // Get the equivalent binary operation for the opcode
                        BinaryExpression.BinaryOperation operation = BinaryExpression.GetBinaryOperation(properties.Code);

                        // Get the precedence of the left and right hand expressions and the current operation
                        int leftPrecedence = BinaryExpression.GetBinaryOperationPrecedence(left), rightPrecedence = BinaryExpression.GetBinaryOperationPrecedence(right);
                        int currentPrecedence = BinaryExpression.GetBinaryOperationPrecedence(operation);

                        // If the left hand expression has a lower precedence than the current operation, we need to wrap it in an expression group
                        if (leftPrecedence < currentPrecedence && leftPrecedence > 0)
                            left = new ExpressionGroup(left);

                        // If the right hand expression has a lower precedence than the current operation, we need to wrap it in an expression group
                        if (rightPrecedence < currentPrecedence && rightPrecedence > 0)
                            right = new ExpressionGroup(right);

                        BinaryExpression binary = new BinaryExpression(left, operation, right);

                        registers.LoadRegister(instruction.A, binary, block, pc);
                        break;
                    }
                    case OpCode.CONCAT:
                    {
                        // Kinda annoying, gotta unravel the whole TING
                        Expression expression = registers.BuildConcat(instruction.B, instruction.C);

                        registers.LoadRegister(instruction.A, expression, block, pc);
                        break;
                    }
                    case OpCode.NOT:
                    case OpCode.LENGTH:
                    case OpCode.MINUS:
                    {
                        var expression = registers.GetExpressionOrThrow(instruction.B);
                        var unaryExpression = new UnaryExpression(expression,
                            UnaryExpression.OperationFromOpCode(properties.Code));
                        registers.LoadRegister(instruction.A, unaryExpression, block, pc);
                        break;
                    }
                    case OpCode.SETTABLEKS:
                    case OpCode.SETUDATAKS:
                    {
                        var aux = function.Instructions[++pc];
                        var target = function.GetConstantOrThrow<StringConstant>((int)aux.Value);

                        if (!registers.TryGetExpression(instruction.B, out var tableExpression))
                            throw new DecompilerException(Stage.Lifter, "Could not find table");

                        if (tableExpression.Expression is TableLiteral tableLiteral && _options.InlineTableDefintions)
                        {
                            if (tableExpression.Declaration.Referenced == 1)
                            {
                                var nestedExpression = registers.GetExpression(instruction.B)?.Expression;
                                if (nestedExpression is not TableLiteral nestedLiteral)
                                    throw new DecompilerException(Stage.Lifter, "Failed to inline table definition");

                                tableLiteral = nestedLiteral;
                            }

                            if (tableLiteral.MaxEntries > tableLiteral.Entries.Count)
                            {
                                tableLiteral.AddEntry(target.ToExpressionGlobal(), registers.GetExpression(instruction.A));
                                break;
                            }
                        }

                        NameIndex nameIndex = new NameIndex(tableExpression, target.Value);

                        block.AddStatement(new Assignment(nameIndex, registers.GetExpression(instruction.A)), pc);
                        break;
                    }
                    case OpCode.SETTABLE:
                    {
                        var expression = registers.GetExpressionOrThrow(instruction.C).Expression;
                        var value = registers.GetExpressionOrThrow(instruction.A);
                        var table = registers.GetExpressionOrThrow(instruction.B, false);

                        if (_options.InlineTableDefintions && table.Expression is TableLiteral tableLiteral)
                        {
                            if (tableLiteral.MaxEntries > tableLiteral.Entries.Count)
                            {
                                tableLiteral.AddEntry(expression, value);
                                break;
                            }
                        }

                        if (expression is NumberLiteral or StringLiteral)
                            expression = new ExpressionIndex(table, expression);

                        block.AddStatement(new Assignment(expression, value), pc);
                        break;
                    }
                    case OpCode.SETTABLEN:
                    {
                        var expression = registers.GetExpressionOrThrow(instruction.B);
                        var numberLiteral = new NumberLiteral(instruction.C + 1);
                        ExpressionIndex index = new ExpressionIndex(expression, numberLiteral);
                        Expression value = registers.GetExpressionOrThrow(instruction.A);

                        block.AddStatement(new Assignment(index, value), pc);
                        break;
                    }
                    case OpCode.NEWTABLE:
                    {
                        // Todo: rewrite this stuff so it all works with 64 bit integers
                        int arraySize = Convert.ToInt32(function.Instructions[++pc].Value);
                        int hashSize = instruction.B == 0 ? 0 : (1 << (instruction.B - 1));

                        TableLiteral expression;

                        if (arraySize > 0)
                            expression = new TableLiteral(arraySize, true);
                        else
                            expression = new TableLiteral(hashSize, false);

                        if (_options.InlineTableDefintions && hashSize > 0)
                            expression.MaxEntries = hashSize;

                        registers.LoadRegister(instruction.A, expression, block, pc);
                        break;
                    }
                    case OpCode.SETLIST:
                    {
                        var tableExpression = registers.GetExpressionOrThrow(instruction.A, false);
                        if (tableExpression.Expression is not TableLiteral tableLiteral)
                            throw new DecompilerException(Stage.Lifter, "Expected TableLiteral");
                        
                        var aux = function.Instructions[pc + 1];

                        for (int slot = 0; slot < instruction.C - 1; slot++)
                        {
                            var key = aux.Value != 1 ? new NumberLiteral(aux.Value + slot) : null;
                            var valueExpression = registers.GetExpressionOrThrow(slot + instruction.B);
                            if (valueExpression.Expression is not { } value)
                                throw new DecompilerException(Stage.Lifter, "");
                            
                            tableLiteral.AddEntry(key!, value);
                        }

                        // Consume AUX
                        pc++;
                        break;
                    }
                    case OpCode.DUPTABLE:
                    {
                        var target = function.GetConstantOrThrow<TableConstant>(instruction.D);

                        TableLiteral tableLiteral = new TableLiteral(target.Value.Count, false);

                        if (_options.InlineTableDefintions)
                            tableLiteral.MaxEntries = target.Value.Count;


                        registers.LoadRegister(instruction.A, tableLiteral, block, pc);
                        break;
                    }
                    case OpCode.GETVARARGS:
                    {
                        int count = instruction.B - 1;

                        if (count > 0)
                        {
                            ExpressionList expressions = new ExpressionList(count);

                            for (int i = 0; i < count; ++i)
                            {
                                int register = instruction.A + i;

                                expressions.Append(registers.LoadTempRegister(register, new Vararg(), block, Declaration.DeclarationType.Local));

                                // All of the variables need to be referenced more than once so that we don't end up 
                                // printing '...' for each of their references.
                                registers.Declarations[register].Referenced = 2;
                            }

                            block.AddStatement(new LocalAssignment(expressions, new Vararg()), pc);
                        }
                        else
                            registers.LoadRegister(instruction.A, new Vararg(), block, pc);
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
                        // Console.WriteLine(instruction.Code.ToString());
                        Expression condition = GetCondition(registers, instruction, function.Instructions[pc + 1], function.Constants, out var auxUsed);

                        // If the next instruction is a JUMPBACK instruction, then we must be dealing with a repeat..until loop.
                        Instruction nextInstruction = function.Instructions[pc + 1];
                        if (nextInstruction.GetProperties().Code == OpCode.JUMPBACK)
                        {
                            // We need to invert the condition, because repeat..until loops are reversed.
                            condition = condition.InvertCondition();

                            Block body = new();

                            // Now we need to take all of the statements created after the inital program counter, and add them to our block.
                            for (int i = pc + nextInstruction.D; i < pc + 1; ++i)
                            {
                                if (block.PcMap.ContainsKey(i))
                                {
                                    var stmt = block.PcMap[i];

                                    block.Statements.Remove(stmt);
                                    block.PcMap.Remove(i);

                                    body.AddStatement(stmt, i);
                                }
                            }

                            registers.FreeRegisters(body);

                            block.AddStatement(new RepeatUntil(condition, body), pc);
                            break;
                        }

                        Registers newRegisters = new Registers(registers);
                        Statement statement = new IfElse(condition, LiftBlock(function, newRegisters, auxUsed ? pc + 2 : pc + 1, (pc += instruction.D) + 1));

                        nextInstruction = function.Instructions[pc];
                        switch (nextInstruction.GetProperties().Code)
                        {
                            case OpCode.JUMP:
                            {
                                IfElse ifElse = (IfElse)statement;

                                newRegisters = new Registers(registers);
                                ifElse.ElseBody = LiftBlock(function, newRegisters, pc + 1, (pc += nextInstruction.D) + 1);

                                Block elseBodyBlock = ifElse.ElseBody as Block; // We know it has to be a block

                                // Little optimization to include elseifs
                                if (elseBodyBlock.Statements.Count == 1 && elseBodyBlock.Statements[0] is IfElse)
                                    ifElse.ElseBody = elseBodyBlock.Statements[0] as IfElse;
                                break;
                            }
                            case OpCode.JUMPBACK:
                            {
                                IfElse ifElse = (IfElse)statement;

                                statement = new WhileLoop(ifElse.Condition, ifElse.IfBody);
                                break;
                            }
                        }

                        // Check if the current instruction is a simple jump instruction
                        if ((properties.Code == OpCode.JUMPIF || properties.Code == OpCode.JUMPIFNOT) && statement is IfElse)
                        {
                            // These are the two instructions that are always used for `and` and `or` statements.
                            // We need to check if the register that the unary operator references is the same as 
                            // the one being reassigned in the body of the if statement.
                            byte register = instruction.A;
                            IfElse ifElse = (IfElse)statement;

                            // If our condition is a unary expression and the body of the if statement is a local assignment
                            // we can go on to check if the unary expression is referencing the same register as the local assignment.
                            if (ifElse.IfBody.Statements.Count == 1
                                && ifElse.IfBody.Statements.First() is LocalAssignment localAssignment)
                            {
                                LocalExpression? expression = localAssignment.Expression as LocalExpression;
                                UnaryExpression? unaryCondition = ifElse.Condition as UnaryExpression;

                                if (expression!.Declaration.Register == register)
                                {
                                    // We know that the current jump instruction represents an `and` or `or` statement.
                                    Expression left = unaryCondition is null ? ifElse.Condition : unaryCondition!.Expression;
                                    Expression right = expression!.Expression;

                                    var operation = unaryCondition is null ? BinaryExpression.BinaryOperation.And : BinaryExpression.BinaryOperation.Or;

                                    registers.LoadRegister(register, new BinaryExpression(left, operation, right), block, pc);
                                    break;
                                }
                            }
                        }

                        block.AddStatement(statement, pc);
                        break;
                    }
                    case OpCode.NEWCLOSURE:
                    case OpCode.DUPCLOSURE:
                    {
                        int functionId = properties.Code == OpCode.DUPCLOSURE ? ((ClosureConstant)function.Constants[instruction.D]).Value : instruction.D;

                        Function newFunction = properties.Code == OpCode.DUPCLOSURE ? function.GlobalFunctions[functionId] : function.GetFunction(functionId);
                        Registers newRegisters = newFunction.CreateRegisters(_options);

                        while (newFunction.Upvalues.Count < newFunction.MaxUpvalues)
                        {
                            Instruction capture = function.Instructions[++pc];
                            var captureProperties = capture.GetProperties();
                            var captureType = (CaptureType)capture.A;

                            if (captureProperties.Code != OpCode.CAPTURE)
                                throw new DecompilerException(Stage.Lifter, $"Expected capture instruction following NEWCLOSURE/DUPCLOSURE (fId: {function.Id}, code: {captureProperties.Code}, pc: {pc})");

                            LocalExpression? expression;

                            switch (captureType)
                            {
                                case CaptureType.Reference:
                                    // We can only have reference capture types when creating a new closure. 
                                    if (properties.Code == OpCode.DUPCLOSURE)
                                        throw new DecompilerException(Stage.Lifter, $"Invalid reference capture type when duplicating closure");

                                    goto case CaptureType.Value;
                                case CaptureType.Value:
                                    var type = _options.RenameUpvalues ? Declaration.DeclarationType.Upvalue : Declaration.DeclarationType.Local;

                                    expression = (LocalExpression)registers.GetExpression(capture.B);
                                    expression.Declaration.Type = type;
                                    break;
                                case CaptureType.Upvalue:
                                    // We've got an existing upvalue
                                    expression = function.Upvalues[capture.B];
                                    break;
                                default:
                                    throw new DecompilerException(Stage.Lifter, $"Unknown capture type {capture.A}");
                            }

                            expression!.Declaration.Referenced++;

                            // Add the upvalue to our new function
                            newFunction.Upvalues.Add(expression);
                        }

                        registers.LoadRegister(instruction.A, new Closure(newRegisters.GetDeclarations(), newFunction.IsVararg, LiftBlock(newFunction, newRegisters), functionId), block, pc);
                        break;
                    }
                    case OpCode.GETUPVAL:
                    {
                        LocalExpression expression = function.Upvalues[instruction.B];

                        registers.LoadRegister(instruction.A, expression, block, pc, expression.Declaration.Type);
                        break;
                    }
                    case OpCode.SETUPVAL:
                    {
                        LocalExpression expression = function.Upvalues[instruction.B];
                        block.AddStatement(new Assignment(expression, registers.GetExpressionOrThrow(instruction.A)), pc);
                        break;
                    }
                    case OpCode.RETURN:
                    {
                        IList<Expression> expressions = new List<Expression>();

                        int numArgs = instruction.B > 0 ? instruction.B - 1 : registers.Top - instruction.A + 1;

                        for (int slot = 0; slot < numArgs; ++slot)
                            expressions.Add(registers.GetExpression(instruction.A + slot));

                        if (pc + 1 < function.Instructions.Count || expressions.Count > 0)
                            block.AddStatement(new Return(expressions), pc);
                        break;
                    }
                    case OpCode.FORNPREP:
                    {
                        var limit = registers.GetExpressionOrThrow(instruction.A);
                        var step = registers.GetExpressionOrThrow(instruction.A + 1);

                        var index = registers.GetExpressionOrThrow(instruction.A + 2);

                        foreach (var statement in block.Statements)
                        {
                            if (statement is LocalAssignment { Expression: LocalExpression localExpression } && localExpression.Declaration.Id == index.Declaration.Id)
                            {
                                block.Statements.Remove(statement);
                                break;
                            }
                        }

                        Assignment assignment = new(new Global(index.Declaration.Name), index.GetValue()!);
                        Block body = LiftBlock(function, registers, pc + 1, pc += instruction.D);

                        block.AddStatement(new ForLoopNumeric(assignment, limit, step, body), pc);
                        break;
                    }
                    case OpCode.FORGPREP:
                    case OpCode.FORGPREP_INEXT:
                    case OpCode.FORGPREP_NEXT:
                    {
                        List<Declaration> variables = new();
                        ExpressionList values = new(3);

                        // Fill expressions. We are assuming the following register format: generator, state, index
                        for (int register = instruction.A; register < 3; ++register)
                        {
                            if (!registers.TryGetExpression(register, out var expression))
                                break;

                            values.Append(expression.GetValue() is NilLiteral ? null : expression);
                        }
                          
                        var loopInstruction = function.Instructions[pc + instruction.D + 1];

                        // Make sure we have a FORGLOOP instruction following.
                        if (loopInstruction.Code != OpCode.FORGLOOP)
                            throw new DecompilerException(Stage.Lifter, "Expected FORGLOOP following for loop body");

                        var varCount = instruction.Code == OpCode.FORGPREP_INEXT ? 2 : function.Instructions[pc + instruction.D + 2].Value;

                        // Load all of the variables with placeholder values at at the third register. The first two are 
                        // reserved for the VM.
                        for (int count = 1; count <= varCount; count++)
                        {
                            int register = count + 2 + instruction.A;

                            registers.LoadTempRegister(register, new NilLiteral(), block, Declaration.DeclarationType.Local);
                            variables.Add(registers.GetDeclaration(register));
                        }

                        // Lift the loop body into a block
                        Block body = LiftBlock(function, registers, pc + 1, (pc += instruction.D) + 1);

                        block.AddStatement(new ForLoopGeneric(variables, values, body), pc);
                        break;
                    }

                    // Instructions that require no code. We have them here so that there are no compiler warnings.
                    case OpCode.PREPVARARGS:
                    case OpCode.CAPTURE:
                    case OpCode.CLOSEUPVALS:
                    case OpCode.FASTCALL1:
                    case OpCode.FASTCALL2:
                    case OpCode.FASTCALL3:
                    case OpCode.FASTCALL2K:
                    case OpCode.FORGLOOP:
                    case OpCode.FORNLOOP:
                    case OpCode.JUMP:
                    case OpCode.JUMPBACK:
                        if (properties.HasAux)
                            pc++;
                        break;

                    default:
                    {
                        Log.Warning("Encountered unhandled code {PropertiesCode}, skipping", properties.Code);

                        // If we don't handle the instruction and it has an auxiliary value, we need to skip it
                        if (properties.HasAux)
                            pc++;

                        break;
                    }
                }
            }

            registers.FreeRegisters(block);

            return block;
        }

        private Expression GetCondition(Registers registers, Instruction instruction, Instruction aux, IList<Constant> constants, out bool auxUsed)
        {
            auxUsed = false;

            OpCode code = instruction.GetProperties().Code;

            switch (code)
            {
                case OpCode.JUMPIF:
                    return new UnaryExpression(registers.GetExpressionOrThrow(instruction.A),
                        UnaryExpression.UnaryOperation.Not);
                case OpCode.JUMPIFNOT:
                    return registers.GetExpressionOrThrow(instruction.A);
            }

            auxUsed = true;

            BinaryExpression.BinaryOperation operation = BinaryExpression.GetBinaryOperation(code);

            if (operation is BinaryExpression.BinaryOperation.CompareGt or BinaryExpression.BinaryOperation.CompareGe)
                return new BinaryExpression(
                    registers.GetExpressionOrThrow((int)aux.Value),
                    operation,
                    registers.GetExpressionOrThrow(instruction.A));

            Expression right;
            switch (code)
            {
                case OpCode.JUMPXEQKNIL:
                    right = new NilLiteral();
                    break;
                case OpCode.JUMPXEQKB:
                    right = new BooleanLiteral((aux.Value & 0x1) == 0x1);
                    break;
                case OpCode.JUMPXEQKN:
                case OpCode.JUMPXEQKS:
                    uint constantIndex = aux.Value & 0xffffff;
                    if (constantIndex >= constants.Count)
                        throw new Exception("Constant value index is greater than constant table size");
                    
                    right = constants[(int)constantIndex].ToExpression();
                    break;
                default:
                    right = registers.GetExpressionOrThrow((int)aux.Value);
                    break;
            }

            return new BinaryExpression(registers.GetExpressionOrThrow(instruction.A), operation, right);
        }
    }
}
