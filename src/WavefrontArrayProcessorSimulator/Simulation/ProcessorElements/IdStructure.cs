using System;
using System.Collections.Generic;

namespace WaveVS.PE
{
    internal struct IdStructure
    {
        public int I { get; }
        public int J { get; }

        public IdStructure(int _i, int _j)
        {
            I = _i; J = _j;
        }
    }
}
