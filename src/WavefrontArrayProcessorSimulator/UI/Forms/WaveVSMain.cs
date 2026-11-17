using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.VSGraphics;
using WaveVS.Services;
using WaveVS.VSGraphics.mainMemory;
using WaveVS.enums;
using WaveVS.WaveFrontVS;
using WaveVS.VSGraphics.codeMemory;
using WaveVS.VSGraphics.PEStructure;
using NLog;
using NLog.Windows.Forms;

namespace WaveVS
{
    public partial class WaveVSMain : Form
    {
        private const int AnimationDelayMilliseconds = 100;
        private const string TASK_FILE_EXT = "shizotex";
        private PETextureGrid _peGrid;
        private MainMemoryTexture _mainMemoryTexture;
        private WaveFrontArrayProcessor _waveFrontArrayProcessor;
        private NumberSystemEnum _memoryDataViewFormat;
        private CodeMemoryTexture _codeMemory;
        private ProcessorElementStructure _processorElementStructure;
        // флаг завершения вычислений ВС
        private bool _calculationDone;

        public WaveVSMain()
        {
            // запрет на сжатие растягивание формы
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            _memoryDataViewFormat = NumberSystemEnum.DEC;
            _calculationDone = false;

            // инициализация граф.компонентов
            InitializeComponent();
            SetupFileDialogs();
            // Размерность процессорного массива по умолчанию.
            WAP_W_Combobox.SelectedIndex = 1;
            WAP_H_Combobox.SelectedIndex = 1;

            // создаем волновой проц
            _waveFrontArrayProcessor = new WaveFrontArrayProcessor();

            // создание текстуры основной памяти
            _mainMemoryTexture = new MainMemoryTexture(
                _waveFrontArrayProcessor.Memory,
                mainMemoryPanel
            );

            // создание текстуры WAP
            _peGrid = new PETextureGrid(WAPanel);

            _codeMemory = new CodeMemoryTexture(CodeTablePanel);
            _codeMemory.InitDefault();
            _waveFrontArrayProcessor.LoadProgram(ProgramEditorAdapter.Read(_codeMemory.CodeLineTextures));

            // создаем текстуру сруктуры ПЭ
            _processorElementStructure = new ProcessorElementStructure(
                PEIdCombobox,
                TopInputBufferLabel,
                LeftInputBufferLabel,
                ExecuteCommandLabel,
                GetMainRegLabels(),
                ResRegLabel,
                PEStateLabel,
                PEStepBtn,
                _peGrid,
                IPLabel,
                CodeViewPanel,
                DownValueLabel,
                RightValueLabel,
                this
            );

            // отрисовка всей ВС волнового фронта
            DrawComputingSystem(
                Convert.ToInt32(((string)WAP_W_Combobox.SelectedItem)),
                Convert.ToInt32(((string)WAP_H_Combobox.SelectedItem))
            );
            SetupNLogConfiguration();
        }

        private void DisableEditing()
        {
            WAP_W_Combobox.Enabled = false;
            WAP_H_Combobox.Enabled = false;
            FileMenuItem.Enabled = false;
            LoadProgramBtn.Enabled = false;
            AddLineBtn.Enabled = false;
            DelLineBtn.Enabled = false;
            CodeTablePanel.Enabled = false;
        }

        private void EnableEditing()
        {
            CodeTablePanel.Enabled = true;
            DelLineBtn.Enabled = true;
            AddLineBtn.Enabled = true;
            LoadProgramBtn.Enabled = true;
            FileMenuItem.Enabled = true;
            WAP_H_Combobox.Enabled = true;
            WAP_W_Combobox.Enabled = true;
        }

        // настроиваем логер
        private void SetupNLogConfiguration()
        {
            // Создаем новую конфигурацию с нуля
            var config = new NLog.Config.LoggingConfiguration();

            // 2. Логгер для формы (RichTextBox)
            var formTarget = new RichTextBoxTarget()
            {
                Name = "logform",
                FormName = "WaveVSMain",
                TargetRichTextBox = richTextBoxLogs,
                Layout = "${longdate} | ${message}",

            };

            // Добавляем цели в конфиг
            config.AddTarget(formTarget);

            // Правила: все уровни логирования пишем во все цели
            config.AddRuleForAllLevels(formTarget);

            // Применяем конфигурацию
            LogManager.Configuration = config;
        }

        // собираем список РОН
        private List<Label> GetMainRegLabels()
        {
            return new List<Label> { 
                R0Label,
                R1Label,
                R2Label,
                R3Label,
                R4Label,
                R5Label,
                R6Label,
                R7Label,
            };
        }

        // Перестроение системы при изменении размерности.
        private void WAP_H_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawComputingSystem(
                Convert.ToInt32(((string)WAP_W_Combobox.SelectedItem)),
                Convert.ToInt32(((string)WAP_H_Combobox.SelectedItem))
            );
        }

        private void WAP_W_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawComputingSystem(
                Convert.ToInt32(((string)WAP_W_Combobox.SelectedItem)),
                Convert.ToInt32(((string)WAP_H_Combobox.SelectedItem))
            );
        }

        // Перестроение вычислительной системы и ее представления.
        private void DrawComputingSystem(int w, int h)
        {
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;
            ResetMenuItem.Enabled = false;
            PEIdCombobox.Enabled = false;
            DisableEditing();

            if (_waveFrontArrayProcessor != null)
            {
                _waveFrontArrayProcessor.InitProcessor(w, h);
                _waveFrontArrayProcessor.LoadProgram(ProgramEditorAdapter.Read(_codeMemory.CodeLineTextures));
            }

            if (_processorElementStructure != null)
            {
                // отрисовка структуры ПЭ
                _processorElementStructure
                    .FillPEIdComboBox(_waveFrontArrayProcessor.ProcessorMatrix);
            }

            if (_peGrid != null)
            {
                // отрисовка волнового процессорного массива
                _peGrid.CreateGrid(
                    w,
                    h,
                    _waveFrontArrayProcessor.ProcessorMatrix
                );
            }

            if (_mainMemoryTexture != null)
            {
                // отрисовка основной памяти
                _mainMemoryTexture.DrawMainMemmoryTexture(
                    _peGrid.GetAllMemoryModules().Count,
                    _memoryDataViewFormat
                );
            }

            EnableEditing();
            PEIdCombobox.Enabled = true;
            ResetMenuItem.Enabled = true;
            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
        }

        // пошаговое выполнение вычислений на уровне ВС
        private void StepMenuItemClick(object sender, EventArgs e)
        {
            _mainMemoryTexture.Disable();
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;

            DisableEditing();

            if (!_calculationDone)
            {
                _waveFrontArrayProcessor.Step();
                _peGrid.UpdateGrid(_waveFrontArrayProcessor.ProcessorMatrix);
                _processorElementStructure.UpdateCurrentStructure(sender, e, false);
            }

            if (_waveFrontArrayProcessor.CheckAllProcessorsStopped())
            {
                _calculationDone = true;

                WAP_W_Combobox.Enabled = true;
                WAP_H_Combobox.Enabled = true;
                FileMenuItem.Enabled = true;
                LoadProgramBtn.Enabled = true;
                AddLineBtn.Enabled = true;
                DelLineBtn.Enabled = true;
                CodeTablePanel.Enabled = true;
                _mainMemoryTexture.Enable();
                MessageBoxEx.Show(this, "done!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
        }

        // сброс ВС в начальное состояние
        private void ResetMenuItemClick(object sender, EventArgs e)
        {
            _mainMemoryTexture.Disable();
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;
            ResetMenuItem.Enabled = false;
            PEIdCombobox.Enabled = false;
            DisableEditing();

            richTextBoxLogs.Clear();
            richTextBoxLogs.ResetText();
            _waveFrontArrayProcessor.Reset();
            _peGrid.UpdateGrid(_waveFrontArrayProcessor.ProcessorMatrix);
            _processorElementStructure.UpdateCurrentStructure(sender, e, false);
            _calculationDone = false;

            EnableEditing();
            PEIdCombobox.Enabled = true;
            ResetMenuItem.Enabled = true;
            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
            _mainMemoryTexture.Enable();
            MessageBoxEx.Show(this, "Reset!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // автоматическое выполнение вычислений
        private async void StartMenuItemClick(object sender, EventArgs e)
        {
            _mainMemoryTexture.Disable();
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;
            PEIdCombobox.Enabled = false;
            DisableEditing();

            do
            {
                _waveFrontArrayProcessor.Step();
                _peGrid.UpdateGrid(_waveFrontArrayProcessor.ProcessorMatrix);
                _processorElementStructure.UpdateCurrentStructure(sender, e, true);
                await Task.Delay(AnimationDelayMilliseconds); // даю время на отрисовку основному потоку формы
            } 
            while (!_waveFrontArrayProcessor.CheckAllProcessorsStopped());
            _calculationDone = true;

            EnableEditing();
            PEIdCombobox.Enabled = true;
            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
            _mainMemoryTexture.Enable();
            MessageBoxEx.Show(this, "done!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // загрузка программы
        private void loadProgramBtnClick(object sender, EventArgs e)
        {
            _mainMemoryTexture.Disable();
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;
            ResetMenuItem.Enabled = false;
            PEIdCombobox.Enabled = false;
            DisableEditing();

            _waveFrontArrayProcessor.LoadProgram(ProgramEditorAdapter.Read(_codeMemory.CodeLineTextures));
            _processorElementStructure.UpdateCurrentStructure(sender, e, false);

            EnableEditing();
            PEIdCombobox.Enabled = true;
            ResetMenuItem.Enabled = true;
            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
            _mainMemoryTexture.Enable();
            MessageBoxEx.Show(this, "Load done!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // добавить строку кода в конец
        private void AddLineBtnClick(object sender, EventArgs e)
        {

            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;

            PEIdCombobox.Enabled = false;
            DisableEditing();

            _codeMemory.AddLineToEnd();

            EnableEditing();
            PEIdCombobox.Enabled = true;

            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;

        }

        // удалить строку с конца
        private void delLineBtnClick(object sender, EventArgs e)
        {

            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;

            PEIdCombobox.Enabled = false;
            DisableEditing();

            _codeMemory.RemoveLineFromEnd();

            EnableEditing();
            PEIdCombobox.Enabled = true;

            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;

        }

        private void PEIdCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBoxLogs.Clear();
            richTextBoxLogs.ResetText();
        }

        private void WAP_W_Combobox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateDimensionInput(WAP_W_Combobox, WAP_W_Combobox_SelectedIndexChanged, e);
        }

        private void WAP_H_Combobox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateDimensionInput(WAP_H_Combobox, WAP_H_Combobox_SelectedIndexChanged, e);
        }
        private void ValidateDimensionInput(ComboBox comboBox, EventHandler selectionChanged, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            string error = !char.IsDigit(e.KeyChar) ? "Можно вводить только цифры!"
                : e.KeyChar > '4' ? "Выход за верхнюю границу диапазона!"
                : e.KeyChar < '1' ? "Выход за нижнюю границу диапазона!" : null;
            if (error == null) return;

            comboBox.SelectedIndexChanged -= selectionChanged;
            e.Handled = true;
            MessageBoxEx.Show(this, error, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            comboBox.SelectedIndexChanged += selectionChanged;
        }
    }
}
