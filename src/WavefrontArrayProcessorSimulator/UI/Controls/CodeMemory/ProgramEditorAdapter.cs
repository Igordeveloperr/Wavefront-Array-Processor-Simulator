using System.Collections.Generic;
using WaveVS.enums;
using WaveVS.WaveFrontVS.PE.command;

namespace WaveVS.VSGraphics.codeMemory
{
    // Translates editor controls once, before loading the processor array.
    internal static class ProgramEditorAdapter
    {
        public static List<MnemonicCommand> Read(List<CodeLineTexture> codeLines)
        {
            var instructions = new List<MnemonicCommand>();
            codeLines.ForEach(line =>
            {
                // no operands
                if (line.OperandComboBox1.SelectedItem is null &&
                    line.OperandComboBox2.SelectedItem is null)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem
                    ));
                }
                // reg
                else if (line.OperandComboBox1.SelectedItem is RegisterOperandEnum &&
                    line.OperandComboBox2.SelectedItem is null)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (RegisterOperandEnum)line.OperandComboBox1.SelectedItem
                    ));
                }
                // buff 
                else if (line.OperandComboBox1.SelectedItem is BufferOperandEnum &&
                    line.OperandComboBox2.SelectedItem is null)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (BufferOperandEnum)line.OperandComboBox1.SelectedItem
                    ));
                }
                // reg - reg
                else if (line.OperandComboBox1.SelectedItem is RegisterOperandEnum &&
                    line.OperandComboBox2.SelectedItem is RegisterOperandEnum)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (RegisterOperandEnum)line.OperandComboBox1.SelectedItem,
                        (RegisterOperandEnum)line.OperandComboBox2.SelectedItem
                    ));
                }
                // reg - buff
                else if (line.OperandComboBox1.SelectedItem is RegisterOperandEnum &&
                    line.OperandComboBox2.SelectedItem is BufferOperandEnum)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (RegisterOperandEnum)line.OperandComboBox1.SelectedItem,
                        (BufferOperandEnum)line.OperandComboBox2.SelectedItem
                    ));
                }
                // buff - reg
                else if (line.OperandComboBox1.SelectedItem is BufferOperandEnum &&
                    line.OperandComboBox2.SelectedItem is RegisterOperandEnum)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (BufferOperandEnum)line.OperandComboBox1.SelectedItem,
                        (RegisterOperandEnum)line.OperandComboBox2.SelectedItem
                    ));
                }
                // buff - buff
                else if (line.OperandComboBox1.SelectedItem is BufferOperandEnum &&
                    line.OperandComboBox2.SelectedItem is BufferOperandEnum)
                {
                    instructions.Add(new MnemonicCommand(
                        line.AddressLabel.Text,
                        (OperationEnum)line.OperationComboBox.SelectedItem,
                        (BufferOperandEnum)line.OperandComboBox1.SelectedItem,
                        (BufferOperandEnum)line.OperandComboBox2.SelectedItem
                    ));
                }
            });
            return instructions;
        }
    }
}
