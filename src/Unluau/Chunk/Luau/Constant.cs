using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// The type of constants avalible.
    /// </summary>
    public enum ConstantType : byte
    {
        /// <summary>
        /// Equivalent to Luau's `nil` keyword.
        /// </summary>
        Nil,

        /// <summary>
        /// True or false value.
        /// </summary>
        Bool,

        /// <summary>
        /// A double. 
        /// </summary>
        Number,

        /// <summary>
        /// Contains an index to the symbol table.
        /// </summary>
        String,

        /// <summary>
        /// An environment variable. 
        /// </summary>
        Import,

        /// <summary>
        /// A precomputed table (usually constructor data).
        /// </summary>
        Table,

        /// <summary>
        /// A precomputed closure.
        /// </summary>
        Closure,

        /// <summary>
        /// A static vector value (x, y, z, w).
        /// </summary>
        Vector
    }

    /// <summary>
    /// The base class for a constant.
    /// </summary>
    public abstract class Constant(ConstantType type)
    {
        /// <summary>
        /// The type of constant we have.
        /// </summary>
        public ConstantType Type => type;

        /// <summary>
        /// Converts the current <see cref="Constant"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override abstract string? ToString();
    }

    /// <summary>
    /// Creates a new constant value in the chunk.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="type">The type of constant.</param>
    /// <param name="value">The value of the constant.</param>
    public class Constant<T>(ConstantType type, T value) : Constant(type)
    {
        /// <summary>
        /// The value of the constant.
        /// </summary>
        public T Value { get; private set; } = value;

        /// <summary>
        /// Returns a string representation of the constant.
        /// </summary>
        public override string? ToString() => Value is null ? "nil" : Value.ToString();
    }

    /// <summary>
    /// Represents the `nil` constant in Luau code.
    /// </summary>
    public class NilConstant() : Constant(ConstantType.Nil)
    {
        public override string ToString() => "nil";
    }

    /// <summary>
    /// Represents a boolean value in Luau code.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    public class BoolConstant(bool value) : Constant<bool>(ConstantType.Bool, value)
    {
        public override string ToString() => Value ? "true" : "false";
    }

    /// <summary>
    /// Represents a number value constant in Luau code.
    /// </summary>
    /// <param name="value">The value of the constant.</param>
    public class NumberConstant(double value) : Constant<double>(ConstantType.Number, value)
    {
    }

    /// <summary>
    /// Represents a string constant in Luau code.
    /// </summary>
    /// <param name="index">The index of the string in the symbol table.</param>
    public class StringConstant(int index) : Constant<int>(ConstantType.String, index)
    {
    }

    /// <summary>
    /// Represents an import constant in luau code.
    /// </summary>
    /// <param name="names">The list of string constants that make up the import.</param>
    public class ImportConstant(StringConstant[] names) : Constant<StringConstant[]>(ConstantType.Import, names)
    {
        public override string ToString() => $"[{string.Join(",", new List<StringConstant>(Value))}]";
    }

    /// <summary>
    /// Represents a table constant in luau code.
    /// </summary>
    /// <param name="keys">List of constants that make up the keys in the table.</param>
    public class TableConstant(Constant[] keys) : Constant<Constant[]>(ConstantType.Table, keys)
    {
        public override string ToString() => $"{{{string.Join(", ", new List<Constant>(Value))}}}";
    }

    /// <summary>
    /// Represents a closure in Luau code (functions).
    /// </summary>
    /// <param name="index">The index of the associated function prototype in the Luau bytecode.</param>
    public class ClosureConstant(int index) : Constant<int>(ConstantType.Closure, index)
    {
    }

    /// <summary>
    /// Represents a vector constant in Luau bytecode.
    /// </summary>
    /// <param name="values">List of floating point numbers.</param>
    public class VectorConstant(float[] values) : Constant<Vector<float>>(ConstantType.Vector, new(values))
    {
    }
}
