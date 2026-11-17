using System;
using System.Collections.Generic;

namespace WaveVS.WaveFrontVS.Task
{
    [Serializable]
    public class MemoryModuleData
    {
        public int QueueSelectedIndex { get; set; }
        public Dictionary<int, float> MemoryData { get; set; }
    }
}
