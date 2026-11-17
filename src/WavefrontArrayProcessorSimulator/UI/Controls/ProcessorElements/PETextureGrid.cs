using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaveVS.PE;

namespace WaveVS.VSGraphics
{
    internal partial class PETextureGrid
    {
        private List<List<PETexture>> _PETextures = new List<List<PETexture>>();
        private List<MemoryModuleTexture> _topMemoryModules = new List<MemoryModuleTexture>();
        private List<MemoryModuleTexture> _bottomMemoryModules = new List<MemoryModuleTexture>();
        private List<MemoryModuleTexture> _leftMemoryModules = new List<MemoryModuleTexture>();
        private List<MemoryModuleTexture> _rightMemoryModules = new List<MemoryModuleTexture>();

        // 4 стрелки для каждого ПЭ
        private List<List<PictureBox>> _rightArrows = new List<List<PictureBox>>(); // Исходящая вправо
        private List<List<PictureBox>> _downArrows = new List<List<PictureBox>>();  // Исходящая вниз
        private List<List<PictureBox>> _leftArrows = new List<List<PictureBox>>();  // Входящая слева
        private List<List<PictureBox>> _upArrows = new List<List<PictureBox>>();    // Входящая сверху

        // Стрелки для модулей памяти
        private List<PictureBox> _memoryModuleRightArrows = new List<PictureBox>();   // Исходящие вправо (от БП к ПЭ)
        private List<PictureBox> _memoryModuleLeftArrows = new List<PictureBox>();    // Исходящие влево (от БП к ПЭ)
        private List<PictureBox> _memoryModuleDownArrows = new List<PictureBox>();    // Исходящие вниз (от БП к ПЭ)
        private List<PictureBox> _memoryModuleUpArrows = new List<PictureBox>();      // Исходящие вверх (от БП к ПЭ)

        private Control _parentControl;
        private int _horizontalSpacing = 30;
        private int _verticalSpacing = 30;
        private int _arrowSpacing = 10;
        private int _cellWidth;
        private int _cellHeight;

        // Отступы для модулей памяти от сетки ПЭ
        private int _memoryModuleSpacing = 30;

        // Размер стрелок (40x10)
        private int _arrowWidth = 30;
        private int _arrowHeight = 10;

        public PETextureGrid(Control parent)
        {
            _parentControl = parent;

            var sampleControl = new PETexture();
            _cellWidth = sampleControl.Width + _horizontalSpacing;
            _cellHeight = sampleControl.Height + _verticalSpacing;
            sampleControl.Dispose();
        }

        public void CreateGrid(int rows, int cols, List<List<ProcessorElement>> processorElements)
        {
            ClearGrid();
            ClearMemoryModules();
            ClearArrows();
            ClearMemoryModuleArrows();

            // Рассчитываем общий размер сетки
            int gridWidth = cols * _cellWidth - _horizontalSpacing;
            int gridHeight = rows * _cellHeight - _verticalSpacing;

            // Рассчитываем начальную позицию для центровки
            int startX = (_parentControl.Width - gridWidth) / 2;
            int startY = (_parentControl.Height - gridHeight) / 2;

            startX = startX > 0 ? startX : 0;
            startY = startY > 0 ? startY : 0;

            // Создаем сетку ПЭ
            for (int i = 0; i < rows; i++)
            {
                var rowControls = new List<PETexture>();

                for (int j = 0; j < cols; j++)
                {
                    var PETexture = new PETexture();

                    int x = startX + j * _cellWidth;
                    int y = startY + i * _cellHeight;

                    PETexture.Location = new Point(x, y);

                    if (processorElements != null &&
                        i < processorElements.Count &&
                        j < processorElements[i].Count)
                    {
                        PETexture.UpdatePE(processorElements[i][j]);
                    }

                    _parentControl.Controls.Add(PETexture);
                    rowControls.Add(PETexture);
                }

                _PETextures.Add(rowControls);
            }

            // Создаем стрелки для каждого ПЭ
            CreateArrows(startX, startY, rows, cols);

            // Создаем модули памяти по периметру
            CreateMemoryModules(startX, startY, rows, cols, gridWidth, gridHeight);

            // Создаем стрелки для модулей памяти
            CreateMemoryModuleArrows(startX, startY, rows, cols, gridWidth, gridHeight);
        }

        public void UpdateGrid(List<List<ProcessorElement>> processorElements)
        {
            for (int i = 0; i < _PETextures.Count && i < processorElements.Count; i++)
            {
                for (int j = 0; j < _PETextures[i].Count && j < processorElements[i].Count; j++)
                {
                    _PETextures[i][j].UpdatePE(processorElements[i][j]);
                }
            }
        }

        public void ClearGrid()
        {
            foreach (var row in _PETextures)
            {
                foreach (var PETexture in row)
                {
                    _parentControl.Controls.Remove(PETexture);
                    PETexture.Dispose();
                }
            }

            _PETextures.Clear();
            ClearMemoryModules();
            ClearArrows();
            ClearMemoryModuleArrows();
        }

        public void RecenterGrid()
        {
            if (_PETextures.Count == 0 || _PETextures[0].Count == 0)
                return;

            int rows = _PETextures.Count;
            int cols = _PETextures[0].Count;

            int gridWidth = cols * _cellWidth - _horizontalSpacing;
            int gridHeight = rows * _cellHeight - _verticalSpacing;

            int startX = (_parentControl.Width - gridWidth) / 2;
            int startY = (_parentControl.Height - gridHeight) / 2;

            startX = startX > 0 ? startX : 0;
            startY = startY > 0 ? startY : 0;

            // Перемещаем все ПЭ
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var PETexture = _PETextures[i][j];
                    int x = startX + j * _cellWidth;
                    int y = startY + i * _cellHeight;
                    PETexture.Location = new Point(x, y);
                }
            }

            // Перемещаем стрелки
            UpdateArrowsPosition(startX, startY, rows, cols);

            // Перемещаем модули памяти
            UpdateMemoryModulesPosition(startX, startY, rows, cols, gridWidth, gridHeight);
        }

        public void SetSpacing(int horizontalSpacing, int verticalSpacing)
        {
            _horizontalSpacing = horizontalSpacing;
            _verticalSpacing = verticalSpacing;

            if (_PETextures.Count > 0 && _PETextures[0].Count > 0)
            {
                var sampleControl = _PETextures[0][0];
                _cellWidth = sampleControl.Width + _horizontalSpacing;
                _cellHeight = sampleControl.Height + _verticalSpacing;
                RecenterGrid();
            }
        }

        public (int horizontal, int vertical) GetSpacing()
        {
            return (_horizontalSpacing, _verticalSpacing);
        }

        public void SetMemoryModuleSpacing(int spacing)
        {
            _memoryModuleSpacing = spacing;
            RecenterGrid();
        }

        public int GetMemoryModuleSpacing()
        {
            return _memoryModuleSpacing;
        }

        public void SetArrowSize(int width, int height)
        {
            _arrowWidth = width;
            _arrowHeight = height;
            RecenterGrid();
        }

        public (int width, int height) GetArrowSize()
        {
            return (_arrowWidth, _arrowHeight);
        }

        public PETexture GetPETexture(int row, int col)
        {
            if (row >= 0 && row < _PETextures.Count &&
                col >= 0 && col < _PETextures[row].Count)
            {
                return _PETextures[row][col];
            }
            return null;
        }

        public MemoryModuleTexture GetTopMemoryModule(int index)
        {
            return index >= 0 && index < _topMemoryModules.Count ? _topMemoryModules[index] : null;
        }

        public MemoryModuleTexture GetBottomMemoryModule(int index)
        {
            return index >= 0 && index < _bottomMemoryModules.Count ? _bottomMemoryModules[index] : null;
        }

        public MemoryModuleTexture GetLeftMemoryModule(int index)
        {
            return index >= 0 && index < _leftMemoryModules.Count ? _leftMemoryModules[index] : null;
        }

        public MemoryModuleTexture GetRightMemoryModule(int index)
        {
            return index >= 0 && index < _rightMemoryModules.Count ? _rightMemoryModules[index] : null;
        }

        public List<MemoryModuleTexture> GetAllMemoryModules()
        {
            var allModules = new List<MemoryModuleTexture>();
            allModules.AddRange(_topMemoryModules);
            allModules.AddRange(_bottomMemoryModules);
            allModules.AddRange(_leftMemoryModules);
            allModules.AddRange(_rightMemoryModules);
            return allModules;
        }

        // Методы для доступа к стрелкам
        public PictureBox GetRightArrow(int row, int col)
        {
            if (row >= 0 && row < _rightArrows.Count &&
                col >= 0 && col < _rightArrows[row].Count)
            {
                return _rightArrows[row][col];
            }
            return null;
        }

        public PictureBox GetDownArrow(int row, int col)
        {
            if (row >= 0 && row < _downArrows.Count &&
                col >= 0 && col < _downArrows[row].Count)
            {
                return _downArrows[row][col];
            }
            return null;
        }

        public PictureBox GetLeftArrow(int row, int col)
        {
            if (row >= 0 && row < _leftArrows.Count &&
                col >= 0 && col < _leftArrows[row].Count)
            {
                return _leftArrows[row][col];
            }
            return null;
        }

        public PictureBox GetUpArrow(int row, int col)
        {
            if (row >= 0 && row < _upArrows.Count &&
                col >= 0 && col < _upArrows[row].Count)
            {
                return _upArrows[row][col];
            }
            return null;
        }
    }
}