// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unluau.Decleration;

namespace Unluau
{
    public class NamerOptions
    {
        public int NamingDepth { get; set; } = 1;
    }

    public class Namer
    {
        private static int _closureCount = 1;

        private Registers registers;

        public Namer(Registers registers)
        { 
            this.registers = registers; 
        }
        
        public Decleration CreateDecleration(int register, Expression expression, Block block, DeclerationType type, bool isRegular = true)
        {
            if (isRegular)
                return createRegularDecleration(register, expression, block, type);

            return createNamedDecleration(register, expression, block, type);
        }

        public void PurifyVariableNames()
        {
            foreach (KeyValuePair<int, Decleration> declPair in new Dictionary<int, Decleration>(registers.GetDeclerationDict()))
            {
                if (declPair.Value is null)
                    continue;

                string name = declPair.Value.Name;

                if (!name.Contains("_v"))
                    continue;

                string realName = name.Split(new string[] { "_v" }, StringSplitOptions.None)[0];

                registers.SetDecleration(declPair.Key, new Decleration(declPair.Value.Register, updateName(realName), declPair.Value.Location));
            }
        }

        private Decleration createRegularDecleration(int register, Expression expression, Block block, DeclerationType type)
        {
            if (expression is Closure)
                return new Decleration(register, block.Statements.Count, DeclerationType.Closure);

            return new Decleration(register, block.Statements.Count, type);
        }

        private Decleration createNamedDecleration(int register, Expression expression, Block block, DeclerationType type)
        {
            string? name = getName(expression);

            if (name != null)
                return new Decleration(register, updateName(name), block.Statements.Count);
            
            return createRegularDecleration(register, expression, block, type);
        }

        private string? getName(Expression expression)
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

        private string updateName(string name)
        {
            var matchList = registers.GetDeclerations()
                .Where(d => d.Name.StartsWith(name + "_v"))
                .ToList();

            name += "_v" + matchList.Count;

            return name;
        }
    }
}
