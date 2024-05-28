using Unluau.Decompile.AST.Statements;
using Unluau.Decompile.AST.Expressions;

namespace Unluau.Decompile.AST
{
    /// <summary>
    /// The recursive visitor pattern used for traversing the AST
    /// </summary>
    public class Visitor
    {
        public virtual bool Visit(Node node) => true;

        /* Statements */
        public virtual bool Visit(Statement node) => Visit(node as Node);
        public virtual bool Visit(Block node) => Visit(node as Statement);

        /* Expressions */
        public virtual bool Visit(Expression node) => Visit(node as Node);
    }
}
