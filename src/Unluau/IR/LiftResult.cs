namespace Unluau.IR
{
    /// <summary>
    /// Represents a summary of the lifted code.
    /// </summary>
    public record LiftResult
    {
        /// <summary>
        /// The total time taken to lift the code.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// The module that was lifted.
        /// </summary>
        public required Module Module { get; set; }
    }
}
