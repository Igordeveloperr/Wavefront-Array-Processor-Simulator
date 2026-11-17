using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal abstract class Command
    {
        protected const int Operand1Index = 0;
        protected const int Operand2Index = 1;

        public abstract void Execute(params object[] parameters);
    }
}
