using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public abstract class Expression
    {
        public abstract void Write(Output output);
        public virtual string[] GetNames()
        {
            return null;
        }
    }
}
