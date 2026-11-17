using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WaveVS.Services;
using WaveVS.VSGraphics;
using WaveVS.VSGraphics.codeMemory;
using WaveVS.VSGraphics.mainMemory;
using WaveVS.WaveFrontVS.Task;

namespace WaveVS
{
    public partial class WaveVSMain
    {
        private void SaveTaskMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void SetupFileDialogs()
        {
            SaveFileDialog.Title = "Сохранение задачи";
            SaveFileDialog.Filter = $"Файл задачи (*.{TASK_FILE_EXT})|*.{TASK_FILE_EXT}";
            SaveFileDialog.FilterIndex = 1; // По умолчанию выбран первый фильтр
            SaveFileDialog.DefaultExt = TASK_FILE_EXT;
            SaveFileDialog.AddExtension = true;
            SaveFileDialog.OverwritePrompt = true; // Спрашивать при перезаписи
            SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            OpenFileDialog.Title = "Открытие задачи";
            OpenFileDialog.Filter = $"Файл задачи (*.{TASK_FILE_EXT})|*.{TASK_FILE_EXT}";
            OpenFileDialog.FilterIndex = 1;
            OpenFileDialog.DefaultExt = TASK_FILE_EXT;
            OpenFileDialog.AddExtension = true;
            OpenFileDialog.CheckFileExists = true; // Проверяем, что файл существует
            OpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void SaveAsTaskMenuItem_Click(object sender, EventArgs e)
        {
            // Показываем диалог и проверяем результат
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // dto 
                    TaskData task = new TaskData
                    {
                        WidthSelectedIndex = WAP_W_Combobox.SelectedIndex,
                        HeightSelectedIndex = WAP_H_Combobox.SelectedIndex,
                        CodeMemoryData = new List<CodeLineData>(),
                        MainMemoryData = new List<MemoryModuleData>(),
                    };

                    // fill
                    _codeMemory.CodeLineTextures.ForEach(line =>
                    {
                        task.CodeMemoryData.Add(
                            new CodeLineData
                            {
                                LineAddress = line.AddressLabel.Text,
                                OperationSelectedIndex = line.OperationComboBox.SelectedIndex,
                                Operand1SelectedIndex = line.OperandComboBox1.SelectedIndex,
                                Operand2SelectedIndex = line.OperandComboBox2.SelectedIndex,
                            }
                        );
                    });

                    MemoryModuleData moduleData;
                    _mainMemoryTexture.MemoryBankTextures.ForEach(bank =>
                    {
                        moduleData = new MemoryModuleData
                        {
                            QueueSelectedIndex = bank.QueueLenCombobox.SelectedIndex,
                            MemoryData = new Dictionary<int, float>()
                        };
                        foreach (var item in bank.MemoryModule.Memory)
                        {
                            moduleData.MemoryData.Add(item.Key, item.Value);
                        }
                        task.MainMemoryData.Add(moduleData);
                    });

                    TaskFileStore.Save(SaveFileDialog.FileName, task);
                    MessageBoxEx.Show(this, "Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBoxEx.Show(this, $"Save error!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenTaskMenuItem_Click(object sender, EventArgs e)
        {
            _mainMemoryTexture.Disable();
            StepMenuItem.Enabled = false;
            StartMenuItem.Enabled = false;
            ResetMenuItem.Enabled = false;
            PEIdCombobox.Enabled = false;
            DisableEditing();

            // Показываем диалог и проверяем результат
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TaskData task = TaskFileStore.Load(OpenFileDialog.FileName);

                    WAP_W_Combobox.SelectedIndex = task.WidthSelectedIndex;
                    WAP_H_Combobox.SelectedIndex = task.HeightSelectedIndex;
                    _mainMemoryTexture.Disable();
                    StepMenuItem.Enabled = false;
                    StartMenuItem.Enabled = false;
                    ResetMenuItem.Enabled = false;
                    PEIdCombobox.Enabled = false;
                    WAP_W_Combobox.Enabled = false;
                    WAP_H_Combobox.Enabled = false;
                    FileMenuItem.Enabled = false;
                    LoadProgramBtn.Enabled = false;
                    AddLineBtn.Enabled = false;
                    DelLineBtn.Enabled = false;
                    CodeTablePanel.Enabled = false;

                    MemoryBankTexture memoryBank;
                    for (int i = 0; i < _mainMemoryTexture.MemoryBankTextures.Count; i++)
                    {
                        memoryBank = _mainMemoryTexture.MemoryBankTextures[i];
                        memoryBank.QueueLenCombobox.SelectedIndex = task.MainMemoryData[i].QueueSelectedIndex;
                        memoryBank.MemoryModule.SetMemory(task.MainMemoryData[i].MemoryData);
                        memoryBank.FillMemory();
                    }

                    int inCount = _codeMemory.CodeLineTextures.Count;
                    for (int i = 1; i < inCount; i++) 
                    {
                        _codeMemory.RemoveLineFromEnd();
                    }
                    RestoreCodeLine(_codeMemory.CodeLineTextures[0], task.CodeMemoryData[0]);
                    for (int i = 1; i < task.CodeMemoryData.Count; i++) 
                    {
                        _codeMemory.AddLineToEnd();
                        RestoreCodeLine(_codeMemory.CodeLineTextures[i], task.CodeMemoryData[i]);
                    }
                    _waveFrontArrayProcessor.LoadProgram(ProgramEditorAdapter.Read(_codeMemory.CodeLineTextures));
                    richTextBoxLogs.Clear();
                    richTextBoxLogs.ResetText();
                    _waveFrontArrayProcessor.Reset();
                    _processorElementStructure.UpdateCurrentStructure(sender, e, false);
                    _calculationDone = false;

                    MessageBoxEx.Show(this, "Open!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception) 
                {
                    MessageBoxEx.Show(this, "Open error!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableEditing();
            PEIdCombobox.Enabled = true;
            ResetMenuItem.Enabled = true;
            StartMenuItem.Enabled = true;
            StepMenuItem.Enabled = true;
            _mainMemoryTexture.Enable();
        }

        private static void RestoreCodeLine(CodeLineTexture line, CodeLineData data)
        {
            line.AddressLabel.Text = data.LineAddress;
            line.OperationComboBox.SelectedIndex = data.OperationSelectedIndex;
            line.OperandComboBox1.SelectedIndex = data.Operand1SelectedIndex;
            line.OperandComboBox2.SelectedIndex = data.Operand2SelectedIndex;
        }
    }
}
