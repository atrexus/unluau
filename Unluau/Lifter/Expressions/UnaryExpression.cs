// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class UnaryExpression : Expression
    {
        public enum UnaryOperation
        {
            Not,
            Len,
            Minus
        }

        public UnaryOperation Operation { get; set; }
        public Expression Expression { get; set; }

        public UnaryExpression(Expression expression, UnaryOperation operation)
        {
            Expression = expression;
            Operation = operation;
        }

        public override void Write(Output output)
        {
            output.Write(UnaryOperationString(Operation));

            if (Operation == UnaryOperation.Not)
                output.Write(" ");

            Expression.Write(output);
        }

        public static string UnaryOperationString(UnaryOperation operation)
        {
            switch (operation)
            {
                case UnaryOperation.Len:
                    return "#";
                case UnaryOperation.Minus:
                    return "-";
                case UnaryOperation.Not:
                    return "not";
            }

            throw new DecompilerException(Stage.Lifter, "'UnaryOperationString' recieved unhandled operation type");
        }
    }
}
