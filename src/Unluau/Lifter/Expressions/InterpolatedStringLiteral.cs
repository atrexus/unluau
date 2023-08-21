// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    public class InterpolatedStringLiteral : StringLiteral
    {
        public IList<Expression> Arguments { get; set; }

        public InterpolatedStringLiteral(string value, IList<Expression> arguments)
            : base(value)
        {
            Arguments = arguments;
        }

        public override void Write(Output output)
        {
            output.Write("`");
            int argumentIndex = 0;

            string[] words = Value.Split(' ');
            for (int i = 0; i < words.Length; ++i)
            {
                string word = words[i];
                if (word[0] == '%')
                {
                    output.Write("{");
                    Arguments[argumentIndex++].Write(output);
                    output.Write("}");
                }
                else
                {
                    output.Write(word);

                    if (i != words.Length - 1)
                    {
                        output.Write(" ");
                    }
                }   
            }   

            output.Write("`");
        }
    }
}
