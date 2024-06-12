namespace Unluau.Decompile.IL
{
    /// <summary>
    /// The main IL program that contains all of the code.
    /// </summary>
    public class Program(Context context, List<Function> functions, int mainIndex) : Node(context)
    {
        /// <summary>
        /// The version of the IL program.
        /// </summary>
        public const string Version = "2.0.0";

        /// <summary>
        /// The control flow of the program.
        /// </summary>
        public List<Function> Functions { get; set; } = functions;

        /// <summary>
        /// The index of the main function.
        /// </summary>
        public int MainIndex { get; set; } = mainIndex;

        /// <inheritdoc/>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var f in Functions)
                    f.Visit(visitor);
            }
        }
    }
}
