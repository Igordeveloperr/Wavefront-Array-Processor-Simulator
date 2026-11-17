using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveVS.Memory;
using WaveVS.PE;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal class SendRightCommand : Command
    {
        private ProcessorElement _processor;
        public SendRightCommand(ProcessorElement processor)
        {
            _processor = processor;
        }

        public override void Execute(params object[] parameters)
        {
            if (parameters.Length == 1)
            {
                if (parameters[Operand1Index] is Register)
                {
                    _processor.RightValue = ((Register)parameters[Operand1Index]).Value;
                }
                else if (parameters[Operand1Index] is InputBuffer)
                {
                    _processor.RightValue = ((InputBuffer)parameters[Operand1Index]).Value;
                }
            }
        }
    }
}
