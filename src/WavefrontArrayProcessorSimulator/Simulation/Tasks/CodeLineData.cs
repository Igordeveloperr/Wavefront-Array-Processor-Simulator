using System;
using System.Collections.Generic;

namespace WaveVS.WaveFrontVS.Task
{
    [Serializable]
    public class CodeLineData
    {
        public string LineAddress {  get; set; }
        public int OperationSelectedIndex { get; set; }
        public int Operand1SelectedIndex { get; set; }
        public int Operand2SelectedIndex { get; set; }
    }
}
