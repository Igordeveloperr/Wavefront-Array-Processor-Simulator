using System;
using System.Collections.Generic;

namespace WaveVS.Memory
{
    internal class InputBuffer
    {
        public float? Value { get; private set; }

        public InputBuffer()
        {
            Value = null;
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
