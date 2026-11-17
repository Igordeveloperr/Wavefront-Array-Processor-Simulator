using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WaveVS.enums;
using WaveVS.enums.attribute;

namespace WaveVS.VSGraphics.codeMemory
{
    internal class CodeLineTexture
    {
        // основные элементы строки кода
        public Label AddressLabel { get { return _addressLabel; } }
        public ComboBox OperationComboBox { get { return _operationComboBox; } }
        public ComboBox OperandComboBox1 { get { return _operandComboBox1; } }
        public ComboBox OperandComboBox2 { get { return _operandComboBox2; } }

        private Label _addressLabel;
        private ComboBox _operationComboBox;
        private ComboBox _operandComboBox1;
        private ComboBox _operandComboBox2;
        private TableLayoutPanel _codePanel;
        private int _lineAddress;

        public CodeLineTexture(int lineAddress, TableLayoutPanel codePanel)
        {
            _codePanel = codePanel;
            _lineAddress = lineAddress;
            InitializeComponents();
            LoadOperations();
        }

        // создаем основные компоненты команды
        private void InitializeComponents()
        {
            _addressLabel = new Label();
            _addressLabel.Text = $"0x{_lineAddress.ToString("X2")}";
            //_addressLabel.AutoSize = true;

            _operationComboBox = new ComboBox();
            _operationComboBox.SelectedIndexChanged += OperationComboBox_SelectedIndexChanged;
            _operationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _operationComboBox.AutoSize = true;

            _operandComboBox1 = new ComboBox();
            _operandComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            _operandComboBox1.AutoSize = true;

            _operandComboBox2 = new ComboBox();
            _operandComboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            _operandComboBox2.AutoSize = true;
        }

        // заполнение списка операций
        private void LoadOperations()
        {
            _operationComboBox.Items.Clear();
            foreach (OperationEnum operation in Enum.GetValues(typeof(OperationEnum)))
            {
                _operationComboBox.Items.Add(operation);
            }

            if (_operationComboBox.Items.Count > 0)
            {
                _operationComboBox.SelectedIndex = 0;
            }
        }

        // событие изменения операции для опеределение возможных операндов операции
        public void OperationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_operationComboBox.SelectedItem is OperationEnum selectedOperation)
            {
                UpdateOperandComboBoxes(selectedOperation);
            }
        }

        // обновление списков операндов
        private void UpdateOperandComboBoxes(OperationEnum operation)
        {
            // Очищаем ComboBox'ы операндов
            _operandComboBox1.Items.Clear();
            _operandComboBox2.Items.Clear();
            _operandComboBox1.Enabled = false;
            _operandComboBox2.Enabled = false;

            // Получаем спецификации операндов для выбранной операции
            var operandSpecs = GetOperandSpecifications(operation);

            // Заполняем первый операнд
            if (operandSpecs.ContainsKey(0))
            {
                _operandComboBox1.Enabled = true;
                var operandTypes = operandSpecs[0];
                foreach (var operand in GetOperandValues(operandTypes))
                {
                    if (operandTypes.First().Name.Equals("LineAddressOperandEnum") &&
                        (int)operand < _codePanel.RowCount)
                    {
                        _operandComboBox1.Items.Add($"0x{(int)operand:X2}");
                    }
                    else if(!operandTypes.First().Name.Equals("LineAddressOperandEnum"))
                    {
                        _operandComboBox1.Items.Add(operand);
                    }
                }

                if (_operandComboBox1.Items.Count > 0)
                {
                    _operandComboBox1.SelectedIndex = 0;
                }
            }

            // Заполняем второй операнд
            if (operandSpecs.ContainsKey(1))
            {
                _operandComboBox2.Enabled = true;
                var operandTypes = operandSpecs[1];
                foreach (var operand in GetOperandValues(operandTypes))
                {
                    _operandComboBox2.Items.Add(operand);
                }

                if (_operandComboBox2.Items.Count > 0)
                {
                    _operandComboBox2.SelectedIndex = 0;
                }
            }
        }

        // получение спецификации по операндам для конкретной операции
        private Dictionary<int, List<Type>> GetOperandSpecifications(OperationEnum operation)
        {
            var result = new Dictionary<int, List<Type>>();
            var fieldInfo = typeof(OperationEnum).GetField(operation.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(OperandSpecAttribute), false);

            foreach (OperandSpecAttribute attr in attributes)
            {
                if (!result.ContainsKey(attr.Position))
                {
                    result[attr.Position] = new List<Type>();
                }

                // Добавляем все типы операндов для этой позиции, используя новый метод
                foreach (var operandType in attr.GetAllOperandTypes())
                {
                    if (!result[attr.Position].Contains(operandType))
                    {
                        result[attr.Position].Add(operandType);
                    }
                }
            }

            // Проверяем наличие атрибута NoOperands
            var noOperandsAttr = fieldInfo.GetCustomAttributes(typeof(NoOperandsAttribute), false);
            if (noOperandsAttr.Any())
            {
                // Для операций без операндов оставляем словарь пустым
                result.Clear();
            }

            return result;
        }

        // получение значений операнда
        private IEnumerable<object> GetOperandValues(List<Type> operandTypes)
        {
            var result = new List<object>();

            foreach (var type in operandTypes)
            {
                if (type.IsEnum)
                {
                    foreach (var value in Enum.GetValues(type))
                    {
                        if (!result.Contains(value))
                        {
                            result.Add(value);
                        }
                    }
                }
            }

            return result;
        }

        // Дополнительный метод для получения текущей выбранной операции
        public OperationEnum? GetSelectedOperation()
        {
            return _operationComboBox.SelectedItem as OperationEnum?;
        }

        // Метод для получения текущих выбранных операндов
        public (object operand1, object operand2) GetSelectedOperands()
        {
            return (_operandComboBox1.SelectedItem, _operandComboBox2.SelectedItem);
        }

        // Метод для установки значений из сохраненного состояния
        public void SetValues(OperationEnum operation, object operand1 = null, object operand2 = null)
        {
            _operationComboBox.SelectedItem = operation;

            if (operand1 != null && _operandComboBox1.Items.Contains(operand1))
            {
                _operandComboBox1.SelectedItem = operand1;
            }

            if (operand2 != null && _operandComboBox2.Items.Contains(operand2))
            {
                _operandComboBox2.SelectedItem = operand2;
            }
        }
    }
}