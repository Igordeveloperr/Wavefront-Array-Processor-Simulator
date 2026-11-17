using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveVS.PE;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal class SendCommand : Command
    {
        private ProcessorElement _processor;
        public SendCommand(ProcessorElement processor)
        {
            _processor = processor;
        }
        public override void Execute(params object[] parameters)
        {
            _processor.OnlySendMessages = true;
        }
    }
}
