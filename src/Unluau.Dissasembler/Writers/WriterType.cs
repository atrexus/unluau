namespace Unluau.Disassembler.Writers
{
    /// <summary>
    /// The type of writer to use.
    /// </summary>
    public enum WriterType
    {
        /// <summary>
        /// Writes the IR in a human-readable format.
        /// </summary>
        IR,

        /// <summary>
        /// Writes the IR to JSON format.
        /// </summary>
        Json
    }
}
