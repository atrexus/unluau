namespace Unluau.IR.ControlFlow.Nodes
{
    /// <summary>
    /// Represents an abstract block in the control flow. An abstract block acts as a "container" for BasicBlocks.
    /// In the analysis phase, reductions are made to the control flow graph by merging BasicBlocks into AbstractBlocks.
    /// </summary>
    public abstract class AbstractBlock : BasicBlock { }
}
