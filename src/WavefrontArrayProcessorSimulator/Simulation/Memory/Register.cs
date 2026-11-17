using System;
using System.Collections.Generic;

namespace WaveVS.Memory
{
    internal class Register
    {
        public float? Value { get; private set; }

        public Register(float? data = 0)
        {
            Value = data;
        }

        public void WriteData(float? data)
        {
            Value = data;
        }

        public void Clear()
        {
            Value = null;
        }
    }
}
