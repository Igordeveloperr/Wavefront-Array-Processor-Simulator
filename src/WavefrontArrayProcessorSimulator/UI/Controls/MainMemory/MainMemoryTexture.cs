using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.enums;
using WaveVS.Memory;

namespace WaveVS.VSGraphics.mainMemory
{
    internal class MainMemoryTexture
    {
        private MainMemory _mainMemory;
        private CustomFlowLayoutPanel _mainMemoryPanel;
        public List<MemoryBankTexture> MemoryBankTextures;

        public MainMemoryTexture(MainMemory mainMemory, CustomFlowLayoutPanel mainMemoryPanel)
        {
            _mainMemory = mainMemory;
            _mainMemoryPanel = mainMemoryPanel;
            MemoryBankTextures = new List<MemoryBankTexture>();
        }

        public void DrawMainMemmoryTexture(int memoryModuleCount, NumberSystemEnum numberSystem)
        {
            MemoryBankTextures.Clear();
            _mainMemoryPanel.Controls.Clear();
            _mainMemoryPanel.AutoScroll = true;
            _mainMemoryPanel.FlowDirection = FlowDirection.LeftToRight;
            _mainMemoryPanel.WrapContents = true;
            _mainMemoryPanel.AutoSize = false;
            _mainMemoryPanel.AutoScrollPosition = new Point(0, 0);
            MemoryBankTexture bank;
            for (int i = 0; i < memoryModuleCount; i++)
            {
                bank = new MemoryBankTexture(
                    i,
                    _mainMemory.MemoryModules[i],
                    numberSystem
                );
                _mainMemoryPanel.Controls.Add(bank);
                MemoryBankTextures.Add(bank);
            }
        }

        public void Enable()
        {
            for (int i = 0; i < _mainMemoryPanel.Controls.Count; i++)
            {
                _mainMemoryPanel.Controls[i].Enabled = true;
            }
        }

        public void Disable()
        {
            for (int i = 0; i < _mainMemoryPanel.Controls.Count; i++)
            {
                _mainMemoryPanel.Controls[i].Enabled = false;
            }
        }
    }
}
