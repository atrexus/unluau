using System.Collections;

namespace Unluau.Decompile.AST.Expressions
{
    /// <summary>
    /// Creates a new instance of <see cref="ExpressionList"/>.
    /// </summary>
    /// <param name="location">The location of the list in the program.</param>
    /// <param name="expressions">A list of expressions.</param>
    public class ExpressionList(Location location, List<Expression> expressions) : Expression(location), IList<Expression>
    {
        private readonly List<Expression> _list = expressions;

        /// <summary>
        /// Creates a new instance of <see cref="ExpressionList"/>.
        /// </summary>
        /// <param name="location">The location of the list in the program.</param>
        public ExpressionList(Location location) : this(location, []) { }

        /// <summary>
        /// Gets/sets an expression at a specific location in the expression list.
        /// </summary>
        /// <param name="index">The index within the list.</param>
        /// <returns>An expression.</returns>
        public Expression this[int index] { get => _list[index]; set => _list[index] = value; }

        /// <summary>
        /// The number of elements in the expression list.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Whether or not the list is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an expression to the list.
        /// </summary>
        /// <param name="item">The expression.</param>
        public void Add(Expression item) => _list.Add(item);

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear() => _list.Clear();

        /// <summary>
        /// Checks to see if an expression list contains an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the expression exists in the list.</returns>
        public bool Contains(Expression item) => _list.Contains(item);

        /// <summary>
        /// Copies the list to the provided array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">The starting index.</param>
        public void CopyTo(Expression[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets the enumerator from the list.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Expression> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Gets the index of the item in the list.
        /// </summary>
        /// <param name="item">The expression.</param>
        /// <returns>The index.</returns>
        public int IndexOf(Expression item) => _list.IndexOf(item);
        
        /// <summary>
        /// Inserts an expression at a given index in the list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The expression.</param>
        public void Insert(int index, Expression item) => _list.Insert(index, item);

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was successfully removed.</returns>
        public bool Remove(Expression item) => _list.Remove(item);

        /// <summary>
        /// Removes the item at a given index.
        /// </summary>
        /// <param name="index">The index to remove at.</param>
        public void RemoveAt(int index) => _list.RemoveAt(index);

        /// <summary>
        /// Gets the enumerator as <see cref="IEnumerable"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
