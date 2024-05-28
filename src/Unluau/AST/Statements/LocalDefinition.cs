using Unluau.Decompile.AST.Expressions;

namespace Unluau.Decompile.AST.Statements
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <param name="variables"></param>
    /// <param name="expressions"></param>
    public class LocalDefinition(Location location, Variable[] variables, ExpressionList expressions) : Statement(location)
    {
    }
}
