// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Decleration
    {
        public static int IdCounter = 1;

        public string Name { get; set; }
        public int Register { get; set; }
        public int Location { get; private set; }
        public int Referenced { get; set; } = 0;

        public Decleration(LocalVariable localVariable)
        {
            Name = localVariable.Name;
            Register = localVariable.Slot;
        }

        public Decleration(int register, int location)
        {
            Name = "var" + IdCounter++;
            Register = register;
            Location = location;
        }

        public Decleration(int register, string name, int location)
        {
            Name = name;
            Register = register;
            Location = location;
        }
    }
}
