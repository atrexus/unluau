using Unluau.IL.Blocks;
using Unluau.IL.Instructions;
using Unluau.IL.Values;
using Unluau.IL.Values.Conditions;
using Index = Unluau.IL.Values.Index;

namespace Unluau.IL
{
    public class Visitor
    {
        public virtual bool Visit(Node node) => true;

        public virtual bool Visit(Program node) => Visit(node as Node);
        public virtual bool Visit(Closure node) => Visit(node as Node);
        public virtual bool Visit(BasicBlock node) => Visit(node as Node);

        public virtual bool Visit(BasicValue node) => Visit(node as Node);
        public virtual bool Visit(Global node) => Visit(node as BasicValue);
        public virtual bool Visit(Variable node) => Visit(node as BasicValue);
        public virtual bool Visit(Reference node) => Visit(node as BasicValue);
        public virtual bool Visit(Index node) => Visit(node as BasicValue);
        public virtual bool Visit(CallResult node) => Visit(node as BasicValue);

        public virtual bool Visit(BasicCondition node) => Visit(node as BasicValue);
        public virtual bool Visit(Equals node) => Visit(node as BasicCondition);

        public virtual bool Visit(Instruction node) => Visit(node as Node);
        public virtual bool Visit(LoadValue node) => Visit(node as Instruction);
        public virtual bool Visit(Call node) => Visit(node as Instruction);
        public virtual bool Visit(GetIndex node) => Visit(node as Instruction);
        public virtual bool Visit(GetIndexSelf node) => Visit(node as Instruction);
        public virtual bool Visit(Move node) => Visit(node as Instruction);
    }
}
