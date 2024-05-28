namespace Unluau.Decompile.AST
{
    /// <summary>
    /// Describes a position in a script.
    /// </summary>
    public struct Position(int line, int column)
    {
        /// <summary>
        /// The line number of the position.
        /// </summary>
        public int Line { get; set; } = line;
        
        /// <summary>
        /// The column number of the position.
        /// </summary>
        public int Column { get; set; } = column;
    }

    /// <summary>
    /// Describes a location of a node in a script.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    public struct Location(Position start, Position end)
    {
        /// <summary>
        /// The start position of the location.
        /// </summary>
        public Position Start { get; set; } = start;
        
        /// <summary>
        /// The end position of the location.
        /// </summary>
        public Position End { get; set; } = end;
    }
}
