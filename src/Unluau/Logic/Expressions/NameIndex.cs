// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class NameIndex : Expression
    {
        public Expression Expression { get; set; }
        public string Name { get; set; }
        public bool IsSelf { get; set; }

        public NameIndex(Expression expression, string name, bool isSelf = false)
        {
            Expression = expression;
            Name = name;
            IsSelf = isSelf;
        }

        public override void Write(Output output)
        {
            // Add null check
            if (Expression == null)
            {
                output.Write("nil");
                return;
            }
    
            Expression.Write(output);
            output.Write((IsSelf ? ":" : ".") + Name);
        }

        public override string[] GetNames()
        {
            string[] names = Expression.GetNames();
            return names.Length <= 0 ? new[] { Name } : names.Append(Name).ToArray();
        }
    }
}
