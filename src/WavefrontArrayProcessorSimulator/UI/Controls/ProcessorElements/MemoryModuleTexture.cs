using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WaveVS.PE;

namespace WaveVS.VSGraphics
{
    /// <summary>
    /// Класс для отрисовки модуля памяти (БП)
    /// </summary>
    internal class MemoryModuleTexture : UserControl
    {
        private const int ModuleWidth = 100;
        private const int ModuleHeight = 50;
        private readonly Font _font = new Font("Arial", 10, FontStyle.Bold);

        private Panel _mainPanel;
        private Label _mainLabel;

        private string _moduleName;
        private int _moduleIndex;

        public Panel MainPanel => _mainPanel;
        public string ModuleName => _moduleName;
        public int ModuleIndex => _moduleIndex;

        public MemoryModuleTexture(string name, int index)
        {
            _moduleName = name;
            _moduleIndex = index;
            InitializeComponents();
            Size = new Size(ModuleWidth, ModuleHeight);
        }

        private void InitializeComponents()
        {
            // Основная панель модуля памяти
            _mainPanel = new Panel
            {
                Size = new Size(ModuleWidth, ModuleHeight),
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            _mainLabel = new Label
            {
                Text = $"{_moduleName}{_moduleIndex}",
                Font = _font,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            _mainPanel.Controls.Add(_mainLabel);
            Controls.Add(_mainPanel);

            // Настройки самого контрола
            BackColor = Color.Transparent;
            DoubleBuffered = true;
        }

        public void UpdateModule(int index)
        {
            _moduleIndex = index;
            _mainLabel.Text = $"{_moduleName}{_moduleIndex}";
        }

        public void UpdateModule(string name, int index)
        {
            _moduleName = name;
            _moduleIndex = index;
            _mainLabel.Text = $"{_moduleName}{_moduleIndex}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _font?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
