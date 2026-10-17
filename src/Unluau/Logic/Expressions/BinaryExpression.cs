// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Format.Models;
using Unluau.Utilities;

namespace Unluau.Logic.Expressions
{
    public class BinaryExpression : Expression
    {
        public enum BinaryOperation 
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            FloorDivide,
            Modulus,
            Power,
            Concat,
            And,
            Or,
            CompareNe,
            CompareEq,
            CompareGe,
            CompareGt,
            CompareLe,
            CompareLt
        }

        public BinaryOperation Operation { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public BinaryExpression(Expression left, BinaryOperation operation, Expression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public override void Write(Output output)
        {
            Left.Write(output);
            output.Write($" {BinaryOperationChar(Operation)} ");
            Right.Write(output);
        }

        public static int GetBinaryOperationPrecedence(Expression expression)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (expression is not LocalExpression { Expression: BinaryExpression binaryExpression })
                return 0;

            return GetBinaryOperationPrecedence(binaryExpression.Operation);
        }

        public static int GetBinaryOperationPrecedence(BinaryOperation operation)
        {
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch (operation)
            {
                case BinaryOperation.And:
                case BinaryOperation.Or:
                    return 1;
                case BinaryOperation.CompareNe:
                case BinaryOperation.CompareEq:
                case BinaryOperation.CompareGe:
                case BinaryOperation.CompareGt:
                case BinaryOperation.CompareLe:
                case BinaryOperation.CompareLt:
                    return 2;
                case BinaryOperation.Add:
                case BinaryOperation.Subtract:
                    return 3;
                case BinaryOperation.Multiply:
                case BinaryOperation.Divide:
                case BinaryOperation.FloorDivide:
                case BinaryOperation.Modulus:
                    return 4;
                case BinaryOperation.Concat:
                    return 5;
                case BinaryOperation.Power:
                    return 6;
                default:
                    return 0;
            }
        }

        public static string BinaryOperationChar(BinaryOperation operation)
        {
            return operation switch
            {
                BinaryOperation.Add => "+",
                BinaryOperation.Subtract => "-",
                BinaryOperation.Multiply => "*",
                BinaryOperation.Divide => "/",
                BinaryOperation.FloorDivide => "//",
                BinaryOperation.Modulus => "%",
                BinaryOperation.Power => "^",
                BinaryOperation.Concat => "...",
                BinaryOperation.CompareNe => "~=",
                BinaryOperation.CompareEq => "==",
                BinaryOperation.CompareGe => ">=",
                BinaryOperation.CompareGt => ">",
                BinaryOperation.CompareLe => "<=",
                BinaryOperation.CompareLt => "<",
                BinaryOperation.And => "and",
                BinaryOperation.Or => "or",
                _ => throw new DecompilerException(Stage.Lifter,
                    "'BinaryOperationChar' got unexpected binary operation")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="DecompilerException"></exception>
        /// <remarks>This is not entirely accurate for JUMPX instructions since they may encode a negation in
        /// their AUX value.</remarks>
        public static BinaryOperation GetBinaryOperation(OpCode code)
        {
            return code switch
            {
                OpCode.ADDK or OpCode.ADD => BinaryOperation.Add,
                OpCode.SUBK or OpCode.SUB or OpCode.SUBRK => BinaryOperation.Subtract,
                OpCode.MULK or OpCode.MUL => BinaryOperation.Multiply,
                OpCode.DIVK or OpCode.DIV or OpCode.DIVRK => BinaryOperation.Divide,
                OpCode.IDIV or OpCode.IDIVK => BinaryOperation.FloorDivide,
                OpCode.MODK or OpCode.MOD => BinaryOperation.Modulus,
                OpCode.POWK or OpCode.POW => BinaryOperation.Power,
                OpCode.CONCAT => BinaryOperation.Concat,
                OpCode.JUMPIFEQ => BinaryOperation.CompareNe,
                OpCode.JUMPIFNOTEQ or OpCode.JUMPXEQKNIL or OpCode.JUMPXEQKB or OpCode.JUMPXEQKN or OpCode.JUMPXEQKS =>
                    BinaryOperation.CompareEq,
                OpCode.JUMPIFLE => BinaryOperation.CompareGe,
                OpCode.JUMPIFLT => BinaryOperation.CompareGt,
                OpCode.JUMPIFNOTLE => BinaryOperation.CompareLe,
                OpCode.JUMPIFNOTLT => BinaryOperation.CompareLt,
                OpCode.ORK or OpCode.OR => BinaryOperation.Or,
                OpCode.ANDK or OpCode.AND => BinaryOperation.And,
                _ => throw new DecompilerException(Stage.Lifter, "'GetBinaryOperation' got unexpected operation code")
            };
        }
    }
}
