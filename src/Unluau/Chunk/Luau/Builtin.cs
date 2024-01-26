using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Unluau.IL.Values;

namespace Unluau.Chunk.Luau
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

    /// <summary>
    /// Represents a builtin function in Luau.
    /// </summary>
    public class Builtin
    {
        public static string IdToName(byte id)
        {
            if (!Enum.IsDefined(typeof(BuiltinFunction), id))
                throw new ArgumentException($"No built-in function matches the id '{id}'");

            var builtinId = (BuiltinFunction)id;

            switch (builtinId)
            {
                case BuiltinFunction.VECTOR: return "Vector3.new";
                default:
                {
                    string[] names = builtinId.ToString().ToLower().Split('_');

                    return string.Join('.', names);
                }
            }
        }
    }
}
