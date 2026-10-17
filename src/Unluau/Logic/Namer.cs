// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic;
using Unluau.Logic.Expressions;
using Unluau.Logic.Statements;

namespace Unluau
{

    public class Namer
    {
        private readonly Registers _registers;

        public Namer(Registers registers)
        { 
            this._registers = registers; 
        }
        
        public Declaration CreateDeclaration(int register, Expression expression, Block block, Declaration.DeclarationType type, bool isRegular = true)
        {
            if (isRegular)
                return CreateRegularDeclaration(register, expression, block, type);

            return CreateNamedDeclaration(register, expression, block, type);
        }

        public void PurifyVariableNames()
        {
            foreach (KeyValuePair<int, Declaration> declPair in new Dictionary<int, Declaration>(_registers.Declarations))
            {
                if (declPair.Value is null)
                    continue;

                string name = declPair.Value.Name;

                if (!name.Contains("_v"))
                    continue;

                string realName = name.Split(new[] { "_v" }, StringSplitOptions.None)[0];

                var declaration =
                    new Declaration(declPair.Value.Register, UpdateName(realName), declPair.Value.Location);
                _registers.Declarations[declPair.Key] = declaration;
            }
        }

        private static Declaration CreateRegularDeclaration(int register, Expression expression, Block block, Declaration.DeclarationType type)
        {
            if (expression is Closure)
                return new Declaration(register, block.Statements.Count, Declaration.DeclarationType.Closure);

            return new Declaration(register, block.Statements.Count, type);
        }

        private Declaration CreateNamedDeclaration(int register, Expression expression, Block block, Declaration.DeclarationType type)
        {
            if (GetName(expression) is { } name)
                return new Declaration(register, UpdateName(name), block.Statements.Count);
            
            return CreateRegularDeclaration(register, expression, block, type);
        }

        private static string? GetName(Expression expression)
        {
            if (expression is NameIndex nameIndex)
                return nameIndex.Name;

            if (expression is not FunctionCall call)
                return null;

            string[] names = call.GetNames();

            // Note: Roblox uses 'game:GetService' to load modules. We can rename variables that contain
            // these accordingly.
            if (names.Length == 3 && names[0] == "game" && names[1] == "GetService")
                return names[2] + "Service";

            return null;
        }

        private string UpdateName(string name)
        {
            var matchList = _registers.GetDeclarations()
                .Where(d => d.Name.StartsWith(name + "_v"))
                .ToList();

            name += "_v" + matchList.Count;

            return name;
        }
    }
}
