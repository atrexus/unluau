using Unluau.IL.Statements;
using Unluau.IL.Statements.Blocks;
using Unluau.IL.Statements.Instructions;
using Unluau.IL.Values;
using Unluau.IL.Values.Binaries;
using Unluau.IL.Values.Conditions;
using Unluau.IL.Values.Unaries;
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
        public virtual bool Visit(Return node) => Visit(node as Instruction);
        public virtual bool Visit(SetIndex node) => Visit(node as Instruction);
        public virtual bool Visit(SetGlobal node) => Visit(node as Instruction);

        // Blocks
        public virtual bool Visit(IfBlock node) => Visit(node as BasicBlock);

        // Values
        public virtual bool Visit(Global node) => Visit(node as BasicValue);
        public virtual bool Visit(Variable node) => Visit(node as BasicValue);
        public virtual bool Visit(Reference node) => Visit(node as BasicValue);
        public virtual bool Visit(Index node) => Visit(node as BasicValue);
        public virtual bool Visit(CallResult node) => Visit(node as BasicValue);
        public virtual bool Visit(BasicCondition node) => Visit(node as BasicValue);
        public virtual bool Visit(Concat concat) => Visit(concat as BasicValue);
        public virtual bool Visit(ClosureValue closure) => Visit(closure as BasicValue);
        public virtual bool Visit(BasicBinary node) => Visit(node as BasicValue);
        public virtual bool Visit(BasicUnary node) => Visit(node as BasicValue);

        // Conditions
        public virtual bool Visit(Equals node) => Visit(node as BasicCondition);
        public virtual bool Visit(NotEquals node) => Visit(node as BasicCondition);
        public virtual bool Visit(Test node) => Visit(node as BasicCondition);
        public virtual bool Visit(NotTest node) => Visit(node as BasicCondition);

        // Unary
        public virtual bool Visit(Not node) => Visit(node as BasicUnary);
        public virtual bool Visit(Length node) => Visit(node as BasicUnary);
        public virtual bool Visit(Minus node) => Visit(node as BasicUnary);

        // Binary
        public virtual bool Visit(Add node) => Visit(node as BasicBinary);
        public virtual bool Visit(Subtract node) => Visit(node as BasicBinary);
        public virtual bool Visit(Multiply node) => Visit(node as BasicBinary);
        public virtual bool Visit(Divide node) => Visit(node as BasicBinary);
        public virtual bool Visit(Modulus node) => Visit(node as BasicBinary);
        public virtual bool Visit(Power node) => Visit(node as BasicBinary);
        public virtual bool Visit(And node) => Visit(node as BasicBinary);
        public virtual bool Visit(Or node) => Visit(node as BasicBinary);
    }
}
