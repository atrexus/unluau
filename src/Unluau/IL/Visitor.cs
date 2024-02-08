using Unluau.IL.Statements;
using Unluau.IL.Statements.Blocks;
using Unluau.IL.Statements.Instructions;
using Unluau.IL.Values;
using Unluau.IL.Values.Conditions;
using Index = Unluau.IL.Values.Index;

namespace Unluau.IL
{
    public class Visitor
    {
        public virtual bool Visit(Node node) => true;

        // Nodes
        public virtual bool Visit(Program node) => Visit(node as Node);
        public virtual bool Visit(Statement node) => Visit(node as Node);
        public virtual bool Visit(BasicValue node) => Visit(node as Node);

        // Statements 
        public virtual bool Visit(Closure node) => Visit(node as Statement);
        public virtual bool Visit(Instruction node) => Visit(node as Statement);
        public virtual bool Visit(BasicBlock node) => Visit(node as Statement);

        // Instructions
        public virtual bool Visit(LoadValue node) => Visit(node as Instruction);
        public virtual bool Visit(Call node) => Visit(node as Instruction);
        public virtual bool Visit(GetIndex node) => Visit(node as Instruction);
        public virtual bool Visit(GetIndexSelf node) => Visit(node as Instruction);
        public virtual bool Visit(Move node) => Visit(node as Instruction);

        // Blocks
        public virtual bool Visit(IfBlock node) => Visit(node as BasicBlock);

        // Values
        public virtual bool Visit(Global node) => Visit(node as BasicValue);
        public virtual bool Visit(Variable node) => Visit(node as BasicValue);
        public virtual bool Visit(Reference node) => Visit(node as BasicValue);
        public virtual bool Visit(Index node) => Visit(node as BasicValue);
        public virtual bool Visit(CallResult node) => Visit(node as BasicValue);
        public virtual bool Visit(BasicCondition node) => Visit(node as BasicValue);

        // Conditions
        public virtual bool Visit(Equals node) => Visit(node as BasicCondition);
    }
}
