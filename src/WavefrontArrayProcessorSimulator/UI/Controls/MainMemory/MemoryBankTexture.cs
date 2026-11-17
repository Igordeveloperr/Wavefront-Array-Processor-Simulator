using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.enums;
using WaveVS.Memory;
using WaveVS.Services;
using WaveVS.Services.dto;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WaveVS.VSGraphics
{
    internal class MemoryBankTexture : UserControl
    {
        private const int MaxQueueLen = 8;
        private Label _memoryBankLabel;
        private DataGridView _memoryBankView;
        private System.Windows.Forms.ComboBox _queueLenCombobox;
        private int _id;
        private MemoryModule _memoryModule;
        private NumberSystemEnum _numberSystem;

        // Флаг для оптимизации
        private bool _isInitialized = false;

        public System.Windows.Forms.ComboBox QueueLenCombobox { get { return this._queueLenCombobox; } }
        public MemoryModule MemoryModule {  get { return this._memoryModule; } }

        public MemoryBankTexture(int id, MemoryModule memoryModule, NumberSystemEnum numberSystem)
        {
            _id = id;
            _memoryModule = memoryModule;
            _numberSystem = numberSystem;
            this.DoubleBuffered = true;

            InitializeUserControl();
            InitializeComponents(numberSystem);
            FillQueueLenCombobox();
            FillMemory();
            _isInitialized = true;
        }

        private void InitializeUserControl()
        {
            this.Size = new Size(440, 190);
            this.AutoScroll = false;
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void InitializeComponents(NumberSystemEnum numberSystem)
        {            
            _memoryBankLabel = new Label();
            _memoryBankLabel.Text = $"Банк памяти {_id}";
            _memoryBankLabel.Font = new Font("Arial", 16);
            _memoryBankLabel.TextAlign = ContentAlignment.MiddleCenter;
            _memoryBankLabel.AutoSize = false;
            _memoryBankLabel.Size = new Size(440, 30);
            _memoryBankLabel.Location = new Point(0, 0); // Указываем позицию

            _queueLenCombobox = new System.Windows.Forms.ComboBox();
            _queueLenCombobox.Size = new Size(38, 20);  // Задаем явный размер
            _queueLenCombobox.Location = new Point((this.Width - _queueLenCombobox.Width) / 2, 35);
            _queueLenCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            _queueLenCombobox.SelectedIndexChanged += QueueComboboxSelectedIndexChenged;
            _queueLenCombobox.KeyPress += QueueLenComboboxKeyPress;

            _memoryBankView = new DataGridView();

            // ВКЛЮЧАЕМ двойную буферизацию для DataGridView
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)
                .SetValue(_memoryBankView, true, null);

            _memoryBankView.Size = new Size(440, 140);
            _memoryBankView.BackgroundColor = Color.White;
            _memoryBankView.Location = new Point(0, 60); // Позиция под Label
            _memoryBankView.Columns.Add("address", "Адрес");
            _memoryBankView.Columns[0].Width = 50;
            _memoryBankView.Columns[0].ReadOnly = true;

            if (numberSystem == NumberSystemEnum.BIN)
            {
                _memoryBankView.Columns.Add("sign", "Знак");
                _memoryBankView.Columns[1].Width = 40;
                _memoryBankView.Columns.Add("exponent", "Порядок");
                _memoryBankView.Columns[2].Width = 100;
                _memoryBankView.Columns.Add("mantis", "Мантиса");
                _memoryBankView.Columns[3].Width = 230;
            }
            else if (numberSystem == NumberSystemEnum.DEC)
            {
                _memoryBankView.Columns.Add("decValue", "Значение");
                //_memoryBankView.Columns[1].Width = 370;
                _memoryBankView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _memoryBankView.EditingControlShowing += ValidateDecValue;
                _memoryBankView.CellEndEdit += WriteDecValue;
            }

            _memoryBankView.RowHeadersVisible = false;
            // запрет на изменение ширины колонок
            _memoryBankView.AllowUserToResizeColumns = false;
            // Отключить изменение высоты заголовков столбцов
            _memoryBankView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // Запретить изменение высоты строк пользователем
            _memoryBankView.AllowUserToResizeRows = false;
            _memoryBankView.AllowUserToAddRows = false;
            // Убрать разделители, которые позволяют изменять размер
            _memoryBankView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            // Установить минимальную и максимальную высоту
            _memoryBankView.RowTemplate.MinimumHeight = 25;
            // Полностью отключить сортировку для всех столбцов
            _memoryBankView.Columns.Cast<DataGridViewColumn>()
                .ToList()
                .ForEach(column => column.SortMode = DataGridViewColumnSortMode.NotSortable);

            // Добавляем контролы непосредственно в UserControl
            this.Controls.Add(_memoryBankLabel);
            this.Controls.Add(_queueLenCombobox);
            this.Controls.Add(_memoryBankView);
        }

        private void QueueLenComboboxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            // только цифры
            if (!char.IsDigit(e.KeyChar))
            {
                _queueLenCombobox.SelectedIndexChanged -= QueueComboboxSelectedIndexChenged;
                e.Handled = true; // Блокируем ввод
                MessageBox.Show(this, "Можно вводить только цифры!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _queueLenCombobox.SelectedIndexChanged += QueueComboboxSelectedIndexChenged;
            }
            else
            {
                int digit = e.KeyChar - 48;
                if (digit > MaxQueueLen)
                {
                    _queueLenCombobox.SelectedIndexChanged -= QueueComboboxSelectedIndexChenged;
                    e.Handled = true; // Блокируем ввод
                    MessageBox.Show(this, "Выход за верхнюю границу диапазона!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _queueLenCombobox.SelectedIndexChanged += QueueComboboxSelectedIndexChenged;
                }
                else if (digit < 1)
                {
                    _queueLenCombobox.SelectedIndexChanged -= QueueComboboxSelectedIndexChenged;
                    e.Handled = true; // Блокируем ввод
                    MessageBox.Show(this, "Выход за нижнюю границу диапазона!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _queueLenCombobox.SelectedIndexChanged += QueueComboboxSelectedIndexChenged;
                }
            }
        }

        // адаптируем память под новую длину очереди
        private void QueueComboboxSelectedIndexChenged(object sender, EventArgs e)
        {
            _memoryModule.UpdateMemory(
                Convert.ToInt32(_queueLenCombobox.SelectedItem)
            );
            FillMemory();
        }

        // метод валидации ячеек при DEC формате данных
        public void ValidateDecValue(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Проверяем, что редактируется нужная колонка (индекс 0, т.к. это первая добавленная колонка)
            if (_memoryBankView.CurrentCell.ColumnIndex == 1 && e.Control is System.Windows.Forms.TextBox textBox)
            {
                textBox.MaxLength = 7; // Максимум 15 символов для float
                // Удаляем предыдущий обработчик, чтобы не навешивать несколько раз
                textBox.KeyPress -= TextBox_KeyPress;
                // Добавляем новый обработчик
                textBox.KeyPress += TextBox_KeyPress;

            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем:
            // - цифры (0-9)
            // - минус (-)
            // - запятую (,) для дробной части
            // - backspace (уже обрабатывается автоматически, но проверяем на всякий случай)
            // - enter (уже обрабатывается автоматически)

            // Проверяем, является ли введенный символ разрешенным
            if (!char.IsDigit(e.KeyChar) &&      // не цифра
                e.KeyChar != ',' &&              // не запятая
                e.KeyChar != '-' &&              // не минус
                !char.IsControl(e.KeyChar))      // не управляющий символ (backspace, enter и т.д.)
            {
                //System.Media.SystemSounds.Beep.Play();
                // Запрещаем ввод
                e.Handled = true;
                MessageBox.Show(this, "Ввод запрещенного символа!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Дополнительная логика для минуса
            if (e.KeyChar == '-' && sender is System.Windows.Forms.TextBox textBox)
            {
                // Разрешаем минус только в начале строки и только один
                if (textBox.SelectionStart != 0)
                {
                    //System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                    MessageBox.Show(this, "Введено больше одного знака или знак находится не в начале строки!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Дополнительная логика для запятой
            if (e.KeyChar == ',' && sender is System.Windows.Forms.TextBox textBox2)
            {
                // Разрешаем только одну запятую
                if (textBox2.Text.Contains(","))
                {
                    //System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                    MessageBox.Show(this, "Можно ввести только одну запятую!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // метод для записи десятичного значения ячейки в память
        public void WriteDecValue(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _memoryBankView.Columns["decValue"].Index)
            {
                var addressCell = _memoryBankView.Rows[e.RowIndex].Cells[e.ColumnIndex-1];
                var valueCell = _memoryBankView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string value = valueCell.Value?.ToString()?.Trim();
                string address = addressCell.Value?.ToString()?.Trim();
                value = value == "-0" ? "tgdgyhd" : value;
                if (float.TryParse(value, out float result))
                {
                    _memoryModule.WriteCell(
                        int.Parse(address.Substring(2), System.Globalization.NumberStyles.HexNumber), 
                        result
                    );
                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                    _memoryModule.WriteCell(
                        int.Parse(address.Substring(2), System.Globalization.NumberStyles.HexNumber),
                        0
                    );
                    valueCell.Value = "0";
                }
            }
        }

        private void SignTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Нажатие клавиши (с информацией о модификаторах Ctrl, Alt, Shift)
            //if (e.KeyCode == Keys.V && e.Control)
            //{
            //    // Пользователь пытается вставить (Ctrl+V)
            //    e.Handled = true; // Запретить вставку
            //    System.Media.SystemSounds.Beep.Play();
            //}
            System.Media.SystemSounds.Beep.Play();

        }

        // заполняем выподающий список для выбора длины очереди и устанавливаем значение по умолчанию
        private void FillQueueLenCombobox()
        {
            for (int i = 1; i <= MaxQueueLen; i++)
            {
                _queueLenCombobox.Items.Add(i);
            }
            _queueLenCombobox.SelectedIndex = 3;
        }

        // заполнение текстуры памяти данными
        public void FillMemory()
        { 
            BinNumber binNumber;
            _memoryBankView.SuspendLayout(); // Приостанавливаем обновление
            _memoryBankView.Rows.Clear();
            // TODO: try catch
            for (int i = _memoryModule.StartAddress; i <= _memoryModule.EndAddress; i++)
            {
                if(_numberSystem == NumberSystemEnum.BIN)
                {
                    binNumber = NumberSystemsService.ConvertDecToBin(
                        _memoryModule.ReadCell(i)
                    );
                    _memoryBankView.Rows.Add(
                        $"0x{i.ToString("X")}",
                        $"{binNumber.Sign}",
                        $"{binNumber.Exponent}",
                        $"{binNumber.Mantis}"
                    );
                }
                else if (_numberSystem == NumberSystemEnum.DEC)
                {
                    _memoryBankView.Rows.Add(
                        $"0x{i.ToString("X")}",
                        $"{_memoryModule.ReadCell(i)}"
                    );  
                }
            }
            _memoryBankView.ResumeLayout(); // Возобновляем
        }

        // Переопределяем OnPaint для оптимизации
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_isInitialized)
            {
                // Базовую отрисовку делаем только один раз
                return;
            }
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_isInitialized)
            {
                // Фон перерисовываем только при изменении
                e.Graphics.Clear(Color.White);
                return;
            }
            base.OnPaintBackground(e);
        }
    }

}
