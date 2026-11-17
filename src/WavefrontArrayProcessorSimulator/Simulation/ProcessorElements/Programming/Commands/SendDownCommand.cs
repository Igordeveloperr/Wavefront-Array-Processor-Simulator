using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveVS.Memory;
using WaveVS.PE;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal class SendDownCommand : Command
    {
        private ProcessorElement _processor;
        public SendDownCommand(ProcessorElement processor)
        {
            _processor = processor;
        }

        public override void Execute(params object[] parameters)
        {
            if (parameters.Length == 1)
            {
                if (parameters[Operand1Index] is Register)
                {
                    _processor.DownValue = ((Register)parameters[Operand1Index]).Value;
                }
                else if (parameters[Operand1Index] is InputBuffer)
                {
                    _processor.DownValue = ((InputBuffer)parameters[Operand1Index]).Value;
                }
            }
        }
    }
}
