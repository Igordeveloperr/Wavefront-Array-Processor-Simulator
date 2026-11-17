using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.enums;

namespace WaveVS.VSGraphics.codeMemory
{
    internal class CodeMemoryTexture
    {
        // константы индексов колонок
        private const int AddressColIndex = 0;
        private const int OperationColIndex = 1;
        private const int Operand1ColIndex = 2;
        private const int Operand2ColIndex = 3;

        private const float RowHeight = 30;
        private const int MaxRowCount = 15;
        private const int DefaultRowCount = 2;
        private const int MinRowCount = 1;

        private TableLayoutPanel _codePanel;
        private List<CodeLineTexture> _codeLineTextures;

        public List<CodeLineTexture> CodeLineTextures {  get { return _codeLineTextures; } }

        public CodeMemoryTexture(TableLayoutPanel codePanel)
        {
            _codePanel = codePanel;
            _codeLineTextures = new List<CodeLineTexture>();
            _codePanel.RowStyles.Clear();
            _codePanel.RowCount = DefaultRowCount;
        }

        private void Test()
        {
            _codeLineTextures.ForEach(codeLine =>
            {
                Console.WriteLine($"{codeLine.AddressLabel.Text} {codeLine.OperationComboBox.SelectedItem} {codeLine.OperandComboBox1.SelectedItem} {codeLine.OperandComboBox2.SelectedItem}");
            });
        }

        // обновляем операнды операций
        public void UpdateLinesOperands(object sender, EventArgs e)
        {
            _codeLineTextures.ForEach(codeLine =>
            {
                codeLine.OperationComboBox_SelectedIndexChanged(sender, e);
            });
        }

        // создаем память коман дефолтной длины
        public void InitDefault()
        {
            CodeLineTexture codeLine;
            _codePanel.SuspendLayout();
            for (int i = 0; i < DefaultRowCount; i++)
            {
                codeLine =
                    new CodeLineTexture(_codeLineTextures.Count, _codePanel);

                _codeLineTextures.Add(codeLine);
                _codePanel.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, RowHeight)
                );

                FillCodeRow(codeLine,i);
            }
            _codePanel.ResumeLayout();
        }

        // добавить линию в конец памяти
        public void AddLineToEnd()
        {
            if (_codePanel.RowCount + 1 <= MaxRowCount)
            {
                _codePanel.SuspendLayout();
                _codePanel.RowCount += 1;
                CodeLineTexture codeLine =
                    new CodeLineTexture(_codeLineTextures.Count, _codePanel);

                _codeLineTextures.Add(codeLine);
                _codePanel.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, RowHeight)
                );

                FillCodeRow(codeLine);
                _codePanel.ResumeLayout();
            }
            else
            {
                MessageBox.Show("Лимит строк!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Test();
        }

        // удаление с конца списка
        public void RemoveLineFromEnd()
        {
            if (_codePanel.RowCount > MinRowCount)
            {
                _codePanel.SuspendLayout();
                // Удаляем контролы из последней строки
                int lastRow = _codePanel.RowCount - 1;
                for (int col = 0; col < _codePanel.ColumnCount; col++)
                {
                    var control = _codePanel.GetControlFromPosition(col, lastRow);
                    if (control != null)
                    {
                        _codePanel.Controls.Remove(control);
                        control.Dispose(); // Если нужно освободить ресурсы
                    }
                }

                // Уменьшаем количество строк
                _codePanel.RowCount--;

                // Удаляем стиль последней строки
                if (_codePanel.RowStyles.Count > _codePanel.RowCount)
                {
                    _codePanel.RowStyles.RemoveAt(_codePanel.RowStyles.Count - 1);
                }
                _codeLineTextures.RemoveAt(_codeLineTextures.Count - 1);
                _codePanel.ResumeLayout();
            }
            else
            {
                MessageBox.Show("Больше строк удалить нельзя!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Test();
        }

        // заполняем строку контролами
        private void FillCodeRow(CodeLineTexture codeLine, int rowIndex = -1)
        {
            int row = rowIndex == -1 ? _codePanel.RowCount - 1 : rowIndex;
            _codePanel.Controls.Add(
                codeLine.AddressLabel,
                AddressColIndex,
                row
            );

            _codePanel.Controls.Add(
                codeLine.OperationComboBox,
                OperationColIndex,
                row
            );

            _codePanel.Controls.Add(
                codeLine.OperandComboBox1,
                Operand1ColIndex,
                row
            );

            _codePanel.Controls.Add(
                codeLine.OperandComboBox2,
                Operand2ColIndex,
                row
            );
        }
    }
}
