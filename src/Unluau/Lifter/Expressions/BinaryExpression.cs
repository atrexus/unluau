// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;

namespace Unluau
{
    public class BinaryExpression : Expression
    {
        public enum BinaryOperation 
        {
            Add,
            Subtract,
            Mulitply,
            Divide,
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

        public static int GetBinaryOperationPrescedence(Expression expression)
        {
            if (expression is LocalExpression local && local.Expression is BinaryExpression binaryExpression)
            {
                return GetBinaryOperationPrescedence(binaryExpression.Operation);
            }
            else
                return 0;
        }

        public static int GetBinaryOperationPrescedence(BinaryOperation operation)
        {
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
                case BinaryOperation.Mulitply:
                case BinaryOperation.Divide:
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
            switch (operation)
            {
                case BinaryOperation.Add:
                    return "+";
                case BinaryOperation.Subtract:
                    return "-";
                case BinaryOperation.Mulitply:
                    return "*";
                case BinaryOperation.Divide:
                    return "/";
                case BinaryOperation.Modulus:
                    return "%";
                case BinaryOperation.Power:
                    return "^";
                case BinaryOperation.Concat:
                    return "...";
                case BinaryOperation.CompareNe:
                    return "~=";
                case BinaryOperation.CompareEq:
                    return "==";
                case BinaryOperation.CompareGe:
                    return ">=";
                case BinaryOperation.CompareGt:
                    return ">";
                case BinaryOperation.CompareLe:
                    return "<=";
                case BinaryOperation.CompareLt:
                    return "<";
                case BinaryOperation.And:
                    return "and";
                case BinaryOperation.Or:
                    return "or";
            }

            throw new DecompilerException(Stage.Lifter, "'BinaryOperationChar' got unexpected binary operation");
        }

        public static BinaryOperation GetBinaryOperation(OpCode code)
        {
            switch (code)
            {
                case OpCode.ADDK:
                case OpCode.ADD:
                    return BinaryOperation.Add;
                case OpCode.SUBK:
                case OpCode.SUB:
                    return BinaryOperation.Subtract;
                case OpCode.MULK:
                case OpCode.MUL:
                    return BinaryOperation.Mulitply;
                case OpCode.DIVK:
                case OpCode.DIV:
                    return BinaryOperation.Divide;
                case OpCode.MODK:
                case OpCode.MOD:
                    return BinaryOperation.Modulus;
                case OpCode.POWK:
                case OpCode.POW:
                    return BinaryOperation.Power;
                case OpCode.CONCAT:
                    return BinaryOperation.Concat;
                case OpCode.JUMPIFEQ:
                    return BinaryOperation.CompareNe;
                case OpCode.JUMPIFNOTEQ:
                case OpCode.JUMPXEQKNIL:
                case OpCode.JUMPXEQKB:
                case OpCode.JUMPXEQKN:
                case OpCode.JUMPXEQKS:
                    return BinaryOperation.CompareEq;
                case OpCode.JUMPIFLE:
                    return BinaryOperation.CompareGe;
                case OpCode.JUMPIFLT:
                    return BinaryOperation.CompareGt;
                case OpCode.JUMPIFNOTLE:
                    return BinaryOperation.CompareLe;
                case OpCode.JUMPIFNOTLT:
                    return BinaryOperation.CompareLt;
                case OpCode.ORK:
                case OpCode.OR:
                    return BinaryOperation.Or;
                case OpCode.ANDK:
                case OpCode.AND:
                    return BinaryOperation.And;
            }

            throw new DecompilerException(Stage.Lifter, "'GetBinaryOperation' got unexpected operation code");
        }
    }
}
