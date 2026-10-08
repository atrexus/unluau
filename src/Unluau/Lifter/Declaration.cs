// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Declaration
    {
        public static int LocalIdCounter = 0;
        public static int ClosureIdCounter = 0;
        public static int UpvalueIdCounter = 0;

        public enum DeclarationType
        {
            Local,
            Closure,
            Upvalue
        }

        private string? _name;

        public string Name
        {
            get
            {
                if (_name is null)
                    return Type switch
                    {
                        DeclarationType.Local => $"var{Id}",
                        DeclarationType.Closure => $"fun{Id}",
                        DeclarationType.Upvalue => $"upval{Id}",
                        _ => throw new Exception("Invalid DeclarationType")
                    };

                return _name;
            }
        }
        public int Register { get; set; }
        public int Location { get; private set; }
        public int Referenced { get; set; } = 0;
        public DeclarationType Type { get; set; }
        public int Id { get; set; }

        public Declaration(LocalVariable localVariable) : this(localVariable.Slot, localVariable.Name, localVariable.StartPc)
        {
        }

        public Declaration(int register, int location, DeclarationType type)
        {
            Register = register;
            Location = location;
            Type = type;

            Id = type switch
            {
                DeclarationType.Local => LocalIdCounter++,
                DeclarationType.Closure => ClosureIdCounter++,
                DeclarationType.Upvalue => UpvalueIdCounter++,
                _ => throw new Exception("Invalid DeclarationType")
            };
        }

        public Declaration(int register, string name, int location) : this(register, location, DeclarationType.Local)
        {
            _name = name;
        }

        public static void ResetCounters()
        {
            LocalIdCounter = 0;
            ClosureIdCounter = 0;
            UpvalueIdCounter = 0;
        }
    }
}
