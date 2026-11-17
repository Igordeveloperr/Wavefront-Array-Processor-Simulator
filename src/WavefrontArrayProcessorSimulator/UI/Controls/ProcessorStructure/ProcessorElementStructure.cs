using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.PE;
using WaveVS.WaveFrontVS.PE.command;
using WaveVS.WaveFrontVS.PE.program.commandPattern;
using WaveVS.WaveFrontVS.PE.program;
using System.Drawing;

namespace WaveVS.VSGraphics.PEStructure
{
    internal class ProcessorElementStructure
    {
        //private readonly char SEPARATOR = char;
        private ComboBox _peIdComboBox;
        private Label _topInputBufferLabel;
        private Label _leftInputBufferLabel;
        private Label _executeCommandLabel;
        private List<Label> _mainRegLabels;
        private Label _resRegLabel;
        private Label _peStateLabel;
        private Button _peStepBtn;
        PETextureGrid _peGrid;
        private Label _ipLabel;
        private FlowLayoutPanel _codeViewPanel;
        private Label _downValueLabel;
        private Label _rightValueLabel;
        private IdStructure _selectedId;

        private bool _isAutoMod = false;
        private List<List<ProcessorElement>> _processorMatrix;
        private ProcessorElement _currentProcessor;
        private Form _owner;

        public ProcessorElementStructure(
            ComboBox peComboBox,
            Label topInputBufferLabel,
            Label leftInputBufferLabel,
            Label executeCommandLabel,
            List<Label> mainRegLabels,
            Label resRegLabel,
            Label peStateLabel,
            Button peStepBtn,
            PETextureGrid peGrid,
            Label ipLabel,
            FlowLayoutPanel codeViewPanel,
            Label downValueLabel,
            Label rightValueLabel,
            Form owner
        )
        {
            _peIdComboBox = peComboBox;
            _peIdComboBox.SelectedIndexChanged += PEIdComboBoxSelectedIndexChanged;
            _peIdComboBox.KeyPress += PeIdComboBoxKeyPress;

            _topInputBufferLabel = topInputBufferLabel;
            _leftInputBufferLabel = leftInputBufferLabel;
            _executeCommandLabel = executeCommandLabel;
            _mainRegLabels = mainRegLabels;
            _resRegLabel = resRegLabel;
            _peStateLabel = peStateLabel;
            _peStepBtn = peStepBtn;
            _peStepBtn.Click += Step;
            _peGrid = peGrid;
            _ipLabel = ipLabel;
            _codeViewPanel = codeViewPanel;
            _downValueLabel = downValueLabel;
            _rightValueLabel = rightValueLabel;
            _owner = owner;
        }

        private void PeIdComboBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            // только цифры
            if (!char.IsDigit(e.KeyChar))
            {
                _peIdComboBox.SelectedIndexChanged -= PEIdComboBoxSelectedIndexChanged;
                e.Handled = true; // Блокируем ввод
                MessageBoxEx.Show(_owner, "Можно вводить только цифры!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _peIdComboBox.SelectedIndexChanged += PEIdComboBoxSelectedIndexChanged;
            }
            else
            {
                int digit = e.KeyChar - 48;
                if(digit > _processorMatrix.Count-1 || digit < 0)
                {
                    _peIdComboBox.SelectedIndexChanged -= PEIdComboBoxSelectedIndexChanged;
                    e.Handled = true; // Блокируем ввод
                    MessageBoxEx.Show(_owner, "Индекс не существует!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _peIdComboBox.SelectedIndexChanged += PEIdComboBoxSelectedIndexChanged;
                }
            }
        }

        // пошаговое выполнение программы ПЭ, когда он находится в состоянии CALC
        private void Step(object sender, EventArgs e)
        {
            _currentProcessor.StepExecuteProgram();
            PEIdComboBoxSelectedIndexChanged(sender, e);
            _peGrid.UpdateGrid(_processorMatrix);
        }

        // заполнение выпадающего списка с id ПЭ
        public void FillPEIdComboBox(List<List<ProcessorElement>> processorMatrix)
        {
            _processorMatrix = processorMatrix;
            _peIdComboBox.Items.Clear();
            processorMatrix.ForEach(line =>
            {
                line.ForEach(processor => {
                    _peIdComboBox.Items
                    .Add($"{processor.Id.I}{processor.Id.J}");
                });
            });
            _peIdComboBox.SelectedIndex = 0;
        }

        // обновляем данные в структуре выбранного ПЭ
        public void UpdateCurrentStructure(object sender, EventArgs e, bool isAutoMod)
        {
            _isAutoMod = isAutoMod;
            PEIdComboBoxSelectedIndexChanged(sender, e);
        }

        // конвертируем строку из списка в ID ПЭ
        private IdStructure ConvertSelectedItemToId()
        {
            //string[] splitId = ((string)_peIdComboBox.SelectedItem).Split();
            string id = (string)_peIdComboBox.SelectedItem;
            return new IdStructure(
                Convert.ToInt32(id[0] - 48),
                Convert.ToInt32(id[1] - 48)
            );
        }

        // получение объекта выбранного ПЭ
        private ProcessorElement GetCurrentProcessor()
        {
            return _processorMatrix
                .SelectMany(row => row)
                .First(processor =>
                    processor.Id.I == _selectedId.I &&
                    processor.Id.J == _selectedId.J
                );
        }

        // заполняем панель с кодом плюс курсор отладки
        private void FillCodeViewPanel(List<string> program, int? ip)
        {
            if (ip != null)
            {
                _codeViewPanel.SuspendLayout();
                _ipLabel.Text = $"0x{ip.Value.ToString("X2")}";

                _codeViewPanel.Controls.Clear();
                string[] splitCmd;
                program.ForEach(cmd =>
                {
                    splitCmd = cmd.Split(' ');
                    Label outCmd = new Label();
                    outCmd.Size = new Size(210, 20);
                    outCmd.TextAlign = ContentAlignment.MiddleLeft;
                    outCmd.Font = new Font("Microsoft Sans Serif", 9);
                    outCmd.Text = cmd;
                    if (Convert.ToInt32(splitCmd[0], 16) == ip)
                    {
                        outCmd.BackColor = Color.Red;
                        outCmd.ForeColor = Color.White;
                    }
                    _codeViewPanel.Controls.Add(outCmd);
                });
                _codeViewPanel.ResumeLayout();
            }
            else
            {
                MessageBox.Show("Program end!");
            }
        }

        // заполнение структуры ПЭ данными
        private void FillStructure()
        {
            _currentProcessor = GetCurrentProcessor();
            _processorMatrix.ForEach(line =>
            {
                line.ForEach(processor =>
                {
                    processor.Program.CurrentId = _selectedId;
                });
            });
            _topInputBufferLabel.Text = $"{_currentProcessor.TopInputBuffer.Value}";
            _leftInputBufferLabel.Text = $"{_currentProcessor.LeftInputBuffer.Value}";
            _peStateLabel.Text = $"Состояние: {_currentProcessor.State}";
            if (_currentProcessor.State == enums.PEStateEnum.CALC && _isAutoMod == false)
            {
                _peStepBtn.Enabled = true;
                //_peIdComboBox.Enabled = true;
                _executeCommandLabel.Text = $"Команда: {_currentProcessor.Program.CurrentCmd}";
            }
            else
            {
                //_peIdComboBox.Enabled = false;
                _peStepBtn.Enabled = false;
            }
            _resRegLabel.Text = $"{_currentProcessor.ResultRegister.Value}";
            for (int i = 0; i < _currentProcessor.RegisterFile.Count; i++)
            {
                _mainRegLabels[i].Text = $"{_currentProcessor.RegisterFile[i].Value}";
            }
            _downValueLabel.Text = _currentProcessor.DownValue == null ? "" :
                $"{_currentProcessor.DownValue}";
            _rightValueLabel.Text = _currentProcessor.RightValue == null ? "" :
                $"{_currentProcessor.RightValue}";
            FillCodeViewPanel(_currentProcessor.Program.CodeView, _currentProcessor.Program.Ip);
        }

        // обновление данных структуры ПЭ
        private void PEIdComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedId = ConvertSelectedItemToId();
            FillStructure();
        }
    }
}
