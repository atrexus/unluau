using Unluau.Common.IR;

namespace Unluau.Disassemble.Lifting
{
    /// <summary>
    /// Represents a summary of the lifted code.
    /// </summary>
    public record Result
    {
        /// <summary>
        /// The version of the disassembler used to lift the code.
        /// </summary>
        public readonly string Version = "2.0.0";

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
