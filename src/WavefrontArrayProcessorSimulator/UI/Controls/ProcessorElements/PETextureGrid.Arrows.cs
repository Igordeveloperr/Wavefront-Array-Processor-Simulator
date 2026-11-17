using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WaveVS.VSGraphics
{
    internal partial class PETextureGrid
    {
        private void CreateMemoryModuleArrows(int gridStartX, int gridStartY, int rows, int cols, int gridWidth, int gridHeight)
        {
            ClearMemoryModuleArrows();

            // Загружаем изображения стрелок
            Image rightArrowImage = LoadArrowImage("RightArrow.png");
            Image downArrowImage = LoadArrowImage("DownArrow.png");
            Image leftArrowImage = LoadArrowImage("LeftArrow.png");
            Image upArrowImage = LoadArrowImage("UpArrow.png");

            int arrowIndex = 0;

            // 1. ВЕРХНИЕ БП (стрелки направлены вниз к верхнему ряду ПЭ)
            for (int j = 0; j < cols; j++)
            {
                var topModule = _topMemoryModules[j];
                var targetPE = _PETextures[0][j]; // Верхний ПЭ напротив БП

                // Выходная стрелка ВНИЗ (от верхнего БП к верхнему ПЭ)
                PictureBox downArrow = CreateArrow(downArrowImage, false);
                int downX = topModule.Location.X + topModule.Width / 2 - _arrowHeight / 2;
                int downY = topModule.Location.Y + topModule.Height; // Отступ от низа БП
                downArrow.Location = new Point(downX, downY);
                downArrow.Size = new Size(_arrowHeight, _arrowWidth); // Вертикальная стрелка: 10x40
                _parentControl.Controls.Add(downArrow);
                downArrow.BringToFront();
                _memoryModuleDownArrows.Add(downArrow);

                // Входная стрелка ВВЕРХ (от верхнего БП к верхнему ПЭ - обратная связь)
                PictureBox upArrow = CreateArrow(upArrowImage, false);
                int upX = downX + _arrowHeight + _arrowSpacing; // Смещаем вправо для отступа
                int upY = downY;
                upArrow.Location = new Point(upX, upY);
                upArrow.Size = new Size(_arrowHeight, _arrowWidth);
                _parentControl.Controls.Add(upArrow);
                upArrow.BringToFront();
                _memoryModuleUpArrows.Add(upArrow);

                arrowIndex++;
            }

            // 2. ПРАВЫЕ БП (стрелки направлены влево к правому столбцу ПЭ)
            for (int i = 0; i < rows; i++)
            {
                var rightModule = _rightMemoryModules[i];
                var targetPE = _PETextures[i][cols - 1]; // Правый ПЭ напротив БП

                // Выходная стрелка ВЛЕВО (от правого БП к правому ПЭ)
                PictureBox leftArrow = CreateArrow(leftArrowImage, true);
                int leftX = rightModule.Location.X - _arrowWidth ; // Отступ от левого края БП
                int leftY = rightModule.Location.Y + rightModule.Height / 2 - _arrowHeight / 2;
                leftArrow.Location = new Point(leftX, leftY);
                leftArrow.Size = new Size(_arrowWidth, _arrowHeight); // Горизонтальная стрелка: 40x10
                _parentControl.Controls.Add(leftArrow);
                leftArrow.BringToFront();
                _memoryModuleLeftArrows.Add(leftArrow);

                // Входная стрелка ВПРАВО (от правого БП к правому ПЭ - обратная связь)
                PictureBox rightArrow = CreateArrow(rightArrowImage, true);
                int rightX = leftX;
                int rightY = leftY + _arrowHeight + _arrowSpacing; // Смещаем вниз для отступа
                rightArrow.Location = new Point(rightX, rightY);
                rightArrow.Size = new Size(_arrowWidth, _arrowHeight);
                _parentControl.Controls.Add(rightArrow);
                rightArrow.BringToFront();
                _memoryModuleRightArrows.Add(rightArrow);

                arrowIndex++;
            }

            // 3. НИЖНИЕ БП (стрелки направлены вверх к нижнему ряду ПЭ)
            for (int j = 0; j < cols; j++)
            {
                var bottomModule = _bottomMemoryModules[j];
                var targetPE = _PETextures[rows - 1][j]; // Нижний ПЭ напротив БП

                // Выходная стрелка ВВЕРХ (от нижнего БП к нижнему ПЭ)
                PictureBox upArrow = CreateArrow(upArrowImage, false);
                int upX = bottomModule.Location.X + bottomModule.Width / 2 - _arrowHeight / 2;
                int upY = bottomModule.Location.Y - _arrowWidth; // Отступ от верха БП
                upArrow.Location = new Point(upX, upY);
                upArrow.Size = new Size(_arrowHeight, _arrowWidth);
                _parentControl.Controls.Add(upArrow);
                upArrow.BringToFront();
                _memoryModuleUpArrows.Add(upArrow);

                // Входная стрелка ВНИЗ (от нижнего БП к нижнему ПЭ - обратная связь)
                PictureBox downArrow = CreateArrow(downArrowImage, false);
                int downX = upX + _arrowHeight + _arrowSpacing; // Смещаем вправо для отступа
                int downY = upY;
                downArrow.Location = new Point(downX, downY);
                downArrow.Size = new Size(_arrowHeight, _arrowWidth);
                _parentControl.Controls.Add(downArrow);
                downArrow.BringToFront();
                _memoryModuleDownArrows.Add(downArrow);

                arrowIndex++;
            }

            // 4. ЛЕВЫЕ БП (стрелки направлены вправо к левому столбцу ПЭ)
            for (int i = 0; i < rows; i++)
            {
                var leftModule = _leftMemoryModules[i];
                var targetPE = _PETextures[i][0]; // Левый ПЭ напротив БП

                // Выходная стрелка ВПРАВО (от левого БП к левому ПЭ)
                PictureBox rightArrow = CreateArrow(rightArrowImage, true);
                int rightX = leftModule.Location.X + leftModule.Width; // Отступ от правого края БП
                int rightY = leftModule.Location.Y + leftModule.Height / 2 - _arrowHeight / 2;
                rightArrow.Location = new Point(rightX, rightY);
                rightArrow.Size = new Size(_arrowWidth, _arrowHeight);
                _parentControl.Controls.Add(rightArrow);
                rightArrow.BringToFront();
                _memoryModuleRightArrows.Add(rightArrow);

                // Входная стрелка ВЛЕВО (от левого БП к левому ПЭ - обратная связь)
                PictureBox leftArrow = CreateArrow(leftArrowImage, true);
                int leftX = rightX;
                int leftY = rightY + _arrowHeight + _arrowSpacing; // Смещаем вниз для отступа
                leftArrow.Location = new Point(leftX, leftY);
                leftArrow.Size = new Size(_arrowWidth, _arrowHeight);
                _parentControl.Controls.Add(leftArrow);
                leftArrow.BringToFront();
                _memoryModuleLeftArrows.Add(leftArrow);

                arrowIndex++;
            }
        }

        private void ClearMemoryModuleArrows()
        {
            ClearArrowList(_memoryModuleRightArrows);
            ClearArrowList(_memoryModuleLeftArrows);
            ClearArrowList(_memoryModuleDownArrows);
            ClearArrowList(_memoryModuleUpArrows);

            _memoryModuleRightArrows.Clear();
            _memoryModuleLeftArrows.Clear();
            _memoryModuleDownArrows.Clear();
            _memoryModuleUpArrows.Clear();
        }

        private void ClearArrowList(List<PictureBox> arrows)
        {
            foreach (var arrow in arrows)
            {
                if (arrow != null)
                {
                    _parentControl.Controls.Remove(arrow);
                    arrow.Dispose();
                }
            }
        }

        private void CreateArrows(int startX, int startY, int rows, int cols)
        {
            // Инициализируем списки стрелок
            _rightArrows = new List<List<PictureBox>>();
            _downArrows = new List<List<PictureBox>>();
            _leftArrows = new List<List<PictureBox>>();
            _upArrows = new List<List<PictureBox>>();

            // Загружаем изображения стрелок
            Image rightArrowImage = LoadArrowImage("RightArrow.png");
            Image downArrowImage = LoadArrowImage("DownArrow.png");
            Image leftArrowImage = LoadArrowImage("LeftArrow.png");
            Image upArrowImage = LoadArrowImage("UpArrow.png");

            // Создаем стрелки для каждого ПЭ
            for (int i = 0; i < rows; i++)
            {
                var rightRow = new List<PictureBox>();
                var downRow = new List<PictureBox>();
                var leftRow = new List<PictureBox>();
                var upRow = new List<PictureBox>();

                for (int j = 0; j < cols; j++)
                {
                    // Рассчитываем базовые координаты центра ячейки
                    int cellCenterX = startX + j * _cellWidth + _cellWidth / 2;
                    int cellCenterY = startY + i * _cellHeight + _cellHeight / 2;

                    // 1. ИСХОДЯЩАЯ стрелка ВПРАВО (от текущего ПЭ к соседу справа)
                    PictureBox rightArrow = null;
                    if (j < cols - 1) // У последнего столбца нет стрелки вправо
                    {
                        rightArrow = CreateArrow(rightArrowImage, true);
                        // Располагаем справа от ПЭ со смещением вверх относительно центра
                        int x = startX + j * _cellWidth + _PETextures[i][j].Width + (_horizontalSpacing - _arrowWidth) / 2;
                        int y = cellCenterY - _arrowHeight - _arrowSpacing; // Смещаем ВВЕРХ от центра
                        rightArrow.Location = new Point(x, y);
                        _parentControl.Controls.Add(rightArrow);
                        rightArrow.BringToFront();
                    }
                    rightRow.Add(rightArrow);

                    // 2. ИСХОДЯЩАЯ стрелка ВНИЗ (от текущего ПЭ к соседу снизу)
                    PictureBox downArrow = null;
                    if (i < rows - 1) // У последней строки нет стрелки вниз
                    {
                        downArrow = CreateArrow(downArrowImage, false);
                        // Располагаем снизу от ПЭ со смещением влево относительно центра
                        int x = cellCenterX - _arrowHeight + _arrowSpacing; // Смещаем ВПРАВО от центра
                        int y = startY + i * _cellHeight + _PETextures[i][j].Height + (_verticalSpacing - _arrowWidth) / 2;
                        downArrow.Location = new Point(x, y);
                        _parentControl.Controls.Add(downArrow);
                        downArrow.BringToFront();
                    }
                    downRow.Add(downArrow);

                    // 3. ВХОДЯЩАЯ стрелка СЛЕВА (от соседа слева к текущему ПЭ)
                    PictureBox leftArrow = null;
                    if (j > 0) // У первого столбца нет стрелки слева
                    {
                        leftArrow = CreateArrow(leftArrowImage, true);
                        // Располагаем слева от ПЭ со смещением вниз относительно центра
                        int x = startX + j * _cellWidth - (_horizontalSpacing + _arrowWidth) / 2;
                        int y = cellCenterY + _arrowSpacing; // Смещаем ВНИЗ от центра
                        leftArrow.Location = new Point(x, y);
                        _parentControl.Controls.Add(leftArrow);
                        leftArrow.BringToFront();
                    }
                    leftRow.Add(leftArrow);

                    // 4. ВХОДЯЩАЯ стрелка СВЕРХУ (от соседа сверху к текущему ПЭ)
                    PictureBox upArrow = null;
                    if (i > 0) // У первой строки нет стрелки сверху
                    {
                        upArrow = CreateArrow(upArrowImage, false);
                        // Располагаем сверху от ПЭ со смещением вправо относительно центра
                        int x = cellCenterX - _arrowHeight - _arrowSpacing; // Смещаем ВЛЕВО от центра
                        int y = startY + i * _cellHeight - (_verticalSpacing + _arrowWidth) / 2;
                        upArrow.Location = new Point(x, y);
                        _parentControl.Controls.Add(upArrow);
                        upArrow.BringToFront();
                    }
                    upRow.Add(upArrow);
                }

                _rightArrows.Add(rightRow);
                _downArrows.Add(downRow);
                _leftArrows.Add(leftRow);
                _upArrows.Add(upRow);
            }
        }

        private PictureBox CreateArrow(Image image, bool isHorizontal)
        {
            int width = isHorizontal ? _arrowWidth : _arrowHeight;
            int height = isHorizontal ? _arrowHeight : _arrowWidth;

            PictureBox arrow = new PictureBox
            {
                Size = new Size(width, height),
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };
            return arrow;
        }

        private Image LoadArrowImage(string fileName)
        {
            return new ArrowImageFactory(_arrowWidth, _arrowHeight).Load(fileName);
        }

        private void ClearArrows()
        {
            ClearArrowGrid(_rightArrows);
            ClearArrowGrid(_downArrows);
            ClearArrowGrid(_leftArrows);
            ClearArrowGrid(_upArrows);

            _rightArrows.Clear();
            _downArrows.Clear();
            _leftArrows.Clear();
            _upArrows.Clear();
        }

        private void ClearArrowGrid(List<List<PictureBox>> arrowGrid)
        {
            foreach (var row in arrowGrid)
            {
                foreach (var arrow in row)
                {
                    if (arrow != null)
                    {
                        _parentControl.Controls.Remove(arrow);
                        arrow.Dispose();
                    }
                }
            }
        }

        private void UpdateArrowsPosition(int startX, int startY, int rows, int cols)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Рассчитываем базовые координаты центра ячейки
                    int cellCenterX = startX + j * _cellWidth + _cellWidth / 2;
                    int cellCenterY = startY + i * _cellHeight + _cellHeight / 2;

                    // Исходящая вправо
                    if (_rightArrows[i][j] != null)
                    {
                        int x = startX + j * _cellWidth + _PETextures[i][j].Width + (_horizontalSpacing - _arrowWidth) / 2;
                        int y = cellCenterY - _arrowHeight - _arrowSpacing; // Смещаем ВВЕРХ от центра
                        _rightArrows[i][j].Location = new Point(x, y);
                    }

                    // Исходящая вниз
                    if (_downArrows[i][j] != null)
                    {
                        int x = cellCenterX - _arrowHeight + _arrowSpacing; // Смещаем ВПРАВО от центра
                        int y = startY + i * _cellHeight + _PETextures[i][j].Height + (_verticalSpacing - _arrowWidth) / 2;
                        _downArrows[i][j].Location = new Point(x, y);
                    }

                    // Входящая слева
                    if (_leftArrows[i][j] != null)
                    {
                        int x = startX + j * _cellWidth - (_horizontalSpacing + _arrowWidth) / 2;
                        int y = cellCenterY + _arrowSpacing; // Смещаем ВНИЗ от центра
                        _leftArrows[i][j].Location = new Point(x, y);
                    }

                    // Входящая сверху
                    if (_upArrows[i][j] != null)
                    {
                        int x = cellCenterX - _arrowHeight - _arrowSpacing; // Смещаем ВЛЕВО от центра
                        int y = startY + i * _cellHeight - (_verticalSpacing + _arrowWidth) / 2;
                        _upArrows[i][j].Location = new Point(x, y);
                    }
                }
            }
        }

        private void UpdateMemoryModuleArrowsPosition(int gridStartX, int gridStartY, int rows, int cols, int gridWidth, int gridHeight)
        {
            int arrowIndex = 0;

            // 1. ВЕРХНИЕ БП
            for (int j = 0; j < cols; j++)
            {
                if (arrowIndex < _memoryModuleDownArrows.Count)
                {
                    var topModule = _topMemoryModules[j];

                    // Выходная стрелка ВНИЗ
                    int downX = topModule.Location.X + topModule.Width / 2 - _arrowHeight / 2;
                    int downY = topModule.Location.Y + topModule.Height + 5;
                    _memoryModuleDownArrows[arrowIndex].Location = new Point(downX, downY);

                    // Входная стрелка ВВЕРХ
                    if (arrowIndex < _memoryModuleUpArrows.Count)
                    {
                        int upX = downX + _arrowHeight + _arrowSpacing;
                        _memoryModuleUpArrows[arrowIndex].Location = new Point(upX, downY);
                    }
                    arrowIndex++;
                }
            }

            // 2. ПРАВЫЕ БП
            for (int i = 0; i < rows; i++)
            {
                if (arrowIndex < _memoryModuleLeftArrows.Count)
                {
                    var rightModule = _rightMemoryModules[i];

                    // Выходная стрелка ВЛЕВО
                    int leftX = rightModule.Location.X - _arrowWidth - 5;
                    int leftY = rightModule.Location.Y + rightModule.Height / 2 - _arrowHeight / 2;
                    _memoryModuleLeftArrows[arrowIndex].Location = new Point(leftX, leftY);

                    // Входная стрелка ВПРАВО
                    if (arrowIndex < _memoryModuleRightArrows.Count)
                    {
                        int rightY = leftY + _arrowHeight + _arrowSpacing;
                        _memoryModuleRightArrows[arrowIndex].Location = new Point(leftX, rightY);
                    }
                    arrowIndex++;
                }
            }

            // 3. НИЖНИЕ БП
            for (int j = 0; j < cols; j++)
            {
                if (arrowIndex < _memoryModuleUpArrows.Count)
                {
                    var bottomModule = _bottomMemoryModules[j];

                    // Выходная стрелка ВВЕРХ
                    int upX = bottomModule.Location.X + bottomModule.Width / 2 - _arrowHeight / 2;
                    int upY = bottomModule.Location.Y - _arrowWidth - 5;
                    _memoryModuleUpArrows[arrowIndex].Location = new Point(upX, upY);

                    // Входная стрелка ВНИЗ
                    if (arrowIndex < _memoryModuleDownArrows.Count)
                    {
                        int downX = upX + _arrowHeight + _arrowSpacing;
                        _memoryModuleDownArrows[arrowIndex].Location = new Point(downX, upY);
                    }
                    arrowIndex++;
                }
            }

            // 4. ЛЕВЫЕ БП
            for (int i = 0; i < rows; i++)
            {
                if (arrowIndex < _memoryModuleRightArrows.Count)
                {
                    var leftModule = _leftMemoryModules[i];

                    // Выходная стрелка ВПРАВО
                    int rightX = leftModule.Location.X + leftModule.Width + 5;
                    int rightY = leftModule.Location.Y + leftModule.Height / 2 - _arrowHeight / 2;
                    _memoryModuleRightArrows[arrowIndex].Location = new Point(rightX, rightY);

                    // Входная стрелка ВЛЕВО
                    if (arrowIndex < _memoryModuleLeftArrows.Count)
                    {
                        int leftY = rightY + _arrowHeight + _arrowSpacing;
                        _memoryModuleLeftArrows[arrowIndex].Location = new Point(rightX, leftY);
                    }
                    arrowIndex++;
                }
            }
        }
    }
}
