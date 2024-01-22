// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    // Note: Ported from Luau source code.
    public enum BuiltinFunction : byte
    {
        NONE = 0,

        // assert()
        ASSERT,

        // math.
        MATH_ABS,
        MATH_ACOS,
        MATH_ASIN,
        MATH_ATAN2,
        MATH_ATAN,
        MATH_CEIL,
        MATH_COSH,
        MATH_COS,
        MATH_DEG,
        MATH_EXP,
        MATH_FLOOR,
        MATH_FMOD,
        MATH_FREXP,
        MATH_LDEXP,
        MATH_LOG10,
        MATH_LOG,
        MATH_MAX,
        MATH_MIN,
        MATH_MODF,
        MATH_POW,
        MATH_RAD,
        MATH_SINH,
        MATH_SIN,
        MATH_SQRT,
        MATH_TANH,
        MATH_TAN,

        // bit32.
        BIT32_ARSHIFT,
        BIT32_BAND,
        BIT32_BNOT,
        BIT32_BOR,
        BIT32_BXOR,
        BIT32_BTEST,
        BIT32_EXTRACT,
        BIT32_LROTATE,
        BIT32_LSHIFT,
        BIT32_REPLACE,
        BIT32_RROTATE,
        BIT32_RSHIFT,

        // type()
        TYPE,

        // string.
        STRING_BYTE,
        STRING_CHAR,
        STRING_LEN,

        // typeof()
        TYPEOF,

        // string.
        STRING_SUB,

        // math.
        MATH_CLAMP,
        MATH_SIGN,
        MATH_ROUND,

        // raw*
        RAWSET,
        RAWGET,
        RAWEQUAL,

        // table.
        TABLE_INSERT,
        TABLE_UNPACK,

        // vector ctor
        VECTOR,

        // bit32.count
        BIT32_COUNTLZ,
        BIT32_COUNTRZ,

        // select(_, ...)
        SELECT_VARARG,

        // rawlen
        RAWLEN,

        // bit32.extract(_, k, k)
        BIT32_EXTRACTK,

        // get/setmetatable
        GETMETATABLE,
        SETMETATABLE,

        // tonumber/tostring
        TONUMBER,
        TOSTRING,

        // bit32.byteswap(n)
        BIT32_BYTESWAP,

        // buffer.
        BUFFER_READI8,
        BUFFER_READU8,
        BUFFER_WRITEU8,
        BUFFER_READI16,
        BUFFER_READU16,
        BUFFER_WRITEU16,
        BUFFER_READI32,
        BUFFER_READU32,
        BUFFER_WRITEU32,
        BUFFER_READF32,
        BUFFER_WRITEF32,
        BUFFER_READF64,
        BUFFER_WRITEF64,
    }

    public class Builtin
    {
        public BuiltinFunction Function { get; set; }
        public Expression Expression { get; set; }

        private Builtin(BuiltinFunction function, Expression expression)
        {
            Function = function;
            Expression = expression;
        }

        public static Builtin FromId(byte id)
        {
            var function = (BuiltinFunction)id;

            return new(function, ToExpression(function));
        }

        private static Expression ToExpression(BuiltinFunction function)
        {
            switch (function)
            {
                case BuiltinFunction.VECTOR:
                    return new NameIndex(new Global("Vector3"), "new");

                default:
                {
                    string[] names = function.ToString().ToLower().Split('_');

                    Expression expression = new Global(names[0]);

                    for (int i = 1; i < names.Length; i++)
                        expression = new NameIndex(expression, names[i]);

                    return expression;
                }
            }
        }
    }
}
