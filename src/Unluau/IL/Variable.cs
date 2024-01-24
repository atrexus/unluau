using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.IL
{
    public struct Variable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// The assigned register slot for the variable.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// The 
        /// </summary>
        public (int, int) PcScope { get; set; }
    }
}
