using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return BinaryOperation.CompareEq;
                case OpCode.JUMPIFLE:
                    return BinaryOperation.CompareGe;
                case OpCode.JUMPIFLT:
                    return BinaryOperation.CompareGt;
                case OpCode.JUMPIFNOTLE:
                    return BinaryOperation.CompareLe;
                case OpCode.JUMPIFNOTLT:
                    return BinaryOperation.CompareLt;
            }

            throw new DecompilerException(Stage.Lifter, "'GetBinaryOperation' got unexpected operation code");
        }
    }
}
