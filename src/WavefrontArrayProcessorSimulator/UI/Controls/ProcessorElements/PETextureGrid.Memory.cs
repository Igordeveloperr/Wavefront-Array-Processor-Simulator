using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WaveVS.VSGraphics
{
    internal partial class PETextureGrid
    {
        private void CreateMemoryModules(int gridStartX, int gridStartY, int rows, int cols, int gridWidth, int gridHeight)
        {
            int globalIndex = 0;
            int totalModules = 2 * rows + 2 * cols;

            for (int i = 0; i < totalModules; i++)
            {
                var module = new MemoryModuleTexture("БП", globalIndex);
                Point moduleLocation;

                if (i < cols)
                {
                    int j = i;
                    int moduleX = gridStartX + j * _cellWidth + (_cellWidth - module.Width) / 2;
                    int moduleY = gridStartY - module.Height - _memoryModuleSpacing;
                    moduleLocation = new Point(moduleX, moduleY);
                    _topMemoryModules.Add(module);
                }
                else if (i < cols + rows)
                {
                    int indexInSide = i - cols;
                    int moduleX = gridStartX + gridWidth + _memoryModuleSpacing;
                    int moduleY = gridStartY + indexInSide * _cellHeight + (_cellHeight - module.Height) / 2;
                    moduleLocation = new Point(moduleX, moduleY);
                    _rightMemoryModules.Add(module);
                }
                else if (i < 2 * cols + rows)
                {
                    int indexInSide = i - cols - rows;
                    int j = cols - 1 - indexInSide;
                    int moduleX = gridStartX + j * _cellWidth + (_cellWidth - module.Width) / 2;
                    int moduleY = gridStartY + gridHeight + _memoryModuleSpacing;
                    moduleLocation = new Point(moduleX, moduleY);
                    _bottomMemoryModules.Add(module);
                }
                else
                {
                    int indexInSide = i - (2 * cols + rows);
                    int iPos = rows - 1 - indexInSide;
                    int moduleX = gridStartX - module.Width - _memoryModuleSpacing;
                    int moduleY = gridStartY + iPos * _cellHeight + (_cellHeight - module.Height) / 2;
                    moduleLocation = new Point(moduleX, moduleY);
                    _leftMemoryModules.Add(module);
                }

                module.Location = moduleLocation;
                _parentControl.Controls.Add(module);
                module.BringToFront();
                globalIndex++;
            }
        }

        private void ClearMemoryModules()
        {
            ClearModuleList(_topMemoryModules);
            ClearModuleList(_bottomMemoryModules);
            ClearModuleList(_leftMemoryModules);
            ClearModuleList(_rightMemoryModules);

            _topMemoryModules.Clear();
            _bottomMemoryModules.Clear();
            _leftMemoryModules.Clear();
            _rightMemoryModules.Clear();
        }

        private void ClearModuleList(List<MemoryModuleTexture> modules)
        {
            foreach (var module in modules)
            {
                _parentControl.Controls.Remove(module);
                module.Dispose();
            }
        }

        private void UpdateMemoryModulesPosition(int gridStartX, int gridStartY, int rows, int cols, int gridWidth, int gridHeight)
        {
            // Обновляем позиции верхних модулей
            for (int j = 0; j < _topMemoryModules.Count && j < cols; j++)
            {
                var module = _topMemoryModules[j];
                int moduleX = gridStartX + j * _cellWidth + (_cellWidth - module.Width) / 2;
                int moduleY = gridStartY - module.Height - _memoryModuleSpacing;
                module.Location = new Point(moduleX, moduleY);
            }

            // Обновляем позиции правых модулей
            for (int i = 0; i < _rightMemoryModules.Count && i < rows; i++)
            {
                var module = _rightMemoryModules[i];
                int moduleX = gridStartX + gridWidth + _memoryModuleSpacing;
                int moduleY = gridStartY + i * _cellHeight + (_cellHeight - module.Height) / 2;
                module.Location = new Point(moduleX, moduleY);
            }

            // Обновляем позиции нижних модулей
            for (int j = 0; j < _bottomMemoryModules.Count && j < cols; j++)
            {
                var module = _bottomMemoryModules[j];
                int moduleX = gridStartX + j * _cellWidth + (_cellWidth - module.Width) / 2;
                int moduleY = gridStartY + gridHeight + _memoryModuleSpacing;
                module.Location = new Point(moduleX, moduleY);
            }

            // Обновляем позиции левых модулей
            for (int i = 0; i < _leftMemoryModules.Count && i < rows; i++)
            {
                var module = _leftMemoryModules[i];
                int moduleX = gridStartX - module.Width - _memoryModuleSpacing;
                int moduleY = gridStartY + i * _cellHeight + (_cellHeight - module.Height) / 2;
                module.Location = new Point(moduleX, moduleY);
            }
            UpdateMemoryModuleArrowsPosition(gridStartX, gridStartY, rows, cols, gridWidth, gridHeight);
        }
    }
}
