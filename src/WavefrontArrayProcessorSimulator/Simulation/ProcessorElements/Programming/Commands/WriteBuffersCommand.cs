using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveVS.Memory;
using WaveVS.PE;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal class WriteBuffersCommand : Command
    {
        private ProcessorElement _processor;
        public WriteBuffersCommand(ProcessorElement processor)
        {
            _processor = processor;   
        }

        public override void Execute(params object[] parameters)
        {
            // если над нами НЕ ПЭ, то есть БП
            if (_processor.UpProcessorElement == null)
            {
                // если очередь не пустая то записываем
                if (!_processor.UpMemoryModule.IsEmpty)
                {
                    _processor.TopInputBuffer
                        .WriteData(_processor.UpMemoryModule.Dequeue());
                }
            }
            else
            {
                _processor.TopInputBuffer.Clear();
            }

            // если слева НЕ ПЭ, то есть БП
            if (_processor.LeftProcessorElement == null)
            {
                // если очередь не пустая то записываем
                if (!_processor.LeftMemoryModule.IsEmpty)
                {
                    _processor.LeftInputBuffer
                        .WriteData(_processor.LeftMemoryModule.Dequeue());
                }
            }
            else
            {
                _processor.LeftInputBuffer.Clear();
            }
        }
    }
}
