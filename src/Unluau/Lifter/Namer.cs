// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unluau.Declaration;

namespace Unluau
{

    public class Namer
    {
        private static int _closureCount = 1;

        private Registers registers;

        public Namer(Registers registers)
        { 
            this.registers = registers; 
        }
        
        public Declaration CreateDeclaration(int register, Expression expression, Block block, DeclarationType type, bool isRegular = true)
        {
            if (isRegular)
                return CreateRegularDeclaration(register, expression, block, type);

            return CreateNamedDeclaration(register, expression, block, type);
        }

        public void PurifyVariableNames()
        {
            foreach (KeyValuePair<int, Declaration> declPair in new Dictionary<int, Declaration>(registers.Declarations))
            {
                if (declPair.Value is null)
                    continue;

                string name = declPair.Value.Name;

                if (!name.Contains("_v"))
                    continue;

                string realName = name.Split(new string[] { "_v" }, StringSplitOptions.None)[0];

                registers.SetDeclaration(declPair.Key, new Declaration(declPair.Value.Register, UpdateName(realName), declPair.Value.Location));
            }
        }

        private Declaration CreateRegularDeclaration(int register, Expression expression, Block block, DeclarationType type)
        {
            if (expression is Closure)
                return new Declaration(register, block.Statements.Count, DeclarationType.Closure);

            return new Declaration(register, block.Statements.Count, type);
        }

        private Declaration CreateNamedDeclaration(int register, Expression expression, Block block, DeclarationType type)
        {
            string? name = GetName(expression);

            if (name != null)
                return new Declaration(register, UpdateName(name), block.Statements.Count);
            
            return CreateRegularDeclaration(register, expression, block, type);
        }

        private string? GetName(Expression expression)
        {
            NameIndex? nameIndex;
            if ((nameIndex = expression as NameIndex) != null)
                return nameIndex.Name;

            FunctionCall? call;
            if ((call = expression as FunctionCall) != null)
            {
                string[] names = call.GetNames();

                // Note: Roblox uses 'game:GetService' to load modules. We can rename variables that contain
                // these accordingly.
                if (names.Length == 3 && names[0] == "game" && names[1] == "GetService")
                    return names[2] + "Service";
            }

            return null;
        }

        private string UpdateName(string name)
        {
            var matchList = registers.GetDeclarations()
                .Where(d => d.Name.StartsWith(name + "_v"))
                .ToList();

            name += "_v" + matchList.Count;

            return name;
        }
    }
}
