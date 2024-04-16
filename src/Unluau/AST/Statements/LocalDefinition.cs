using Unluau.AST.Expressions;

namespace Unluau.AST.Statements
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
