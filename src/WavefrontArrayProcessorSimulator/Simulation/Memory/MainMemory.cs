using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVS.Memory
{
    internal class MainMemory
    {
        private List<MemoryModule> _memoryModules;
        private int _memoryModuleCount;

        public List<MemoryModule> MemoryModules { get { return _memoryModules; } }

        public MainMemory(int memoryModuleCount)
        {
            _memoryModuleCount = memoryModuleCount;
            _memoryModules = new List<MemoryModule>();
            InitMemoryArray(memoryModuleCount);
        }
        
        // сброс всех очередей в исходное состояние
        public void ResetMemoryQueues()
        {
            _memoryModules.ForEach(module =>
            {
                module.ResetQueue();
            });
        }

        // блочная генерация основной памяти
        private void InitMemoryArray(int memoryModuleCount)
        {
            _memoryModuleCount = memoryModuleCount;
            for (int i = 0; i < _memoryModuleCount; i++)
            {
                _memoryModules.Add(new MemoryModule(i));
            }
        }
    }
}
