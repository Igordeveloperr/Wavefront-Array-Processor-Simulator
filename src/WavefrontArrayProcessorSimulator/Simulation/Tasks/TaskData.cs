using System;
using System.Collections.Generic;

namespace WaveVS.WaveFrontVS.Task
{
    [Serializable]
    public class TaskData
    {
        public int WidthSelectedIndex { get; set; }
        public int HeightSelectedIndex { get; set; }
        public List<MemoryModuleData> MainMemoryData { get; set; }
        public List<CodeLineData> CodeMemoryData { get; set; }
    }
}
