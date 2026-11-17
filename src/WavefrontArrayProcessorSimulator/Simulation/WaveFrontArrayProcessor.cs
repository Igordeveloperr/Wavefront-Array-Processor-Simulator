using System.Collections.Generic;
using System.Linq;
using WaveVS.enums;
using WaveVS.Memory;
using WaveVS.PE;
using WaveVS.WaveFrontVS.PE.command;

namespace WaveVS.WaveFrontVS
{
    internal class WaveFrontArrayProcessor
    {
        private const int MainMemorySize = 16;
        private readonly List<List<ProcessorElement>> _processorMatrix;
        private readonly MainMemory _mainMemory;
        private readonly List<List<int>> _memoryConnectionMatrix;

        public MainMemory Memory { get { return _mainMemory; } }
        public List<List<ProcessorElement>> ProcessorMatrix {  get { return _processorMatrix; } }

        public WaveFrontArrayProcessor()
        {
            _processorMatrix = new List<List<ProcessorElement>>();
            _memoryConnectionMatrix = new List<List<int>>();
            _mainMemory = new MainMemory(MainMemorySize);

        }

        // Загрузка программы процессорных элементов.
        public void LoadProgram(List<MnemonicCommand> instructions)
        {
            foreach (var row in _processorMatrix)
                foreach (var processor in row)
                    processor.LoadProgram(instructions);
        }

        // создание волнового процессора
        public void InitProcessor(int rowsCount, int columsCount)
        {

            _processorMatrix.Clear();
            _memoryConnectionMatrix.Clear();
            // заполнение марицы ПЭ
            for (int i = 0; i < rowsCount; i++)
            {
                var row = new List<ProcessorElement>();
                for (int j = 0; j < columsCount; j++)
                {
                    // создаем ПЭ
                    ProcessorElement processorElement = new ProcessorElement(
                        new IdStructure(i, j)
                    );
                    row.Add(processorElement);
                }
                _processorMatrix.Add(row);
            }

            // создаю матрицу подключений БП
            InitMemoryConnectionMatrix(rowsCount, columsCount);
            // создаем все связи
            InitConnections(rowsCount, columsCount);
        }

        // проверяем остановились ли все ПЭ
        public bool CheckAllProcessorsStopped()
        {
            return _processorMatrix.All(row => row.All(processor => processor.State == PEStateEnum.STOP));
        }

        // пошаговое выполнение на уровне ВС
        public void Step()
        {
            for (int i = 0; i < _processorMatrix.Count; i++)
            {
                for (int j = 0; j < _processorMatrix[i].Count; j++)
                {
                    _processorMatrix[i][j].ExecuteProgram();
                    _processorMatrix[i][j].Program.Reset();
                }
            }
        }

        // сброс ВС в исходное состояние
        public void Reset()
        {
            _mainMemory.ResetMemoryQueues();
            foreach (var row in _processorMatrix)
                foreach (var processor in row)
                    processor.Reset();
        }

        /*
            создаем связи между ПЭ и БП
         */
        private void InitConnections(int rowsCount, int columsCount)
        {
            ProcessorElement processorElement;
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columsCount; j++)
                {
                    processorElement = _processorMatrix[i][j];

                    /* Подключение соседних ПЭ */
                    // левый сосед если есть
                    processorElement.LeftProcessorElement = (j > 0) ? 
                        _processorMatrix[i][j - 1] : null;

                    // правый сосед если есть
                    processorElement.RightProcessorElement = (j < _processorMatrix[i].Count - 1) ?
                        _processorMatrix[i][j + 1] : null;

                    // верхний сосед если есть
                    processorElement.UpProcessorElement = (i > 0) ? 
                        _processorMatrix[i - 1][j] : null;

                    // нижний сосед если есть
                    processorElement.DownProcessorElement = (i < _processorMatrix.Count - 1) ? 
                        _processorMatrix[i + 1][j] : null;

                    /*Подключение модулей памяти*/
                    // если нет левого соседа ПЭ, значит должен быть БП
                    processorElement.LeftMemoryModule = processorElement.LeftProcessorElement == null ?
                        _mainMemory.MemoryModules[
                            _memoryConnectionMatrix[i + 1][j] - 1
                        ]
                        : null;

                    // если нет справа соседа ПЭ, значит должен быть БП
                    processorElement.RightMemoryModule = processorElement.RightProcessorElement == null ?
                        _mainMemory.MemoryModules[
                            _memoryConnectionMatrix[i + 1][j + 2] - 1
                        ]
                        : null;

                    // если нет сверху соседа ПЭ, значит должен быть БП
                    processorElement.UpMemoryModule = processorElement.UpProcessorElement == null ?
                        _mainMemory.MemoryModules[
                            _memoryConnectionMatrix[i][j + 1] - 1
                        ]
                        : null;

                    // если нет снизу соседа ПЭ, значит должен быть БП
                    processorElement.DownMemoryModule = processorElement.DownProcessorElement == null ?
                        _mainMemory.MemoryModules[
                            _memoryConnectionMatrix[i + 2][j + 1] - 1
                        ]
                        : null;

                }
            }
        }

        // создаем матрицу подключений БП к матрице ПЭ
        private void InitMemoryConnectionMatrix(int rowsCount, int columsCount)
        {
            for (int i = 0; i < rowsCount + 2; i++)
            {
                var line = new List<int>();
                for (int j = 0; j < columsCount + 2; j++)
                {
                    line.Add(0);
                }
                _memoryConnectionMatrix.Add(line);
            }

            int memoryModuleIndex = 1;
            // 1. Верхняя сторона (слева направо, без первого и последнего)
            for (int i = 1; i < _memoryConnectionMatrix[0].Count - 1; i++)
            {
                _memoryConnectionMatrix[0][i] = memoryModuleIndex;
                memoryModuleIndex++;
            }

            // 2. Правая сторона (сверху вниз, без первого и последнего)
            for (int i = 1; i < _memoryConnectionMatrix.Count - 1; i++)
            {
                _memoryConnectionMatrix[i][_memoryConnectionMatrix[0].Count-1] = memoryModuleIndex;
                memoryModuleIndex++;
            }

            // 3. Нижняя сторона (справа налево, без первого и последнего)
            for (int i = _memoryConnectionMatrix[0].Count-2; i > 0; i--)
            {
                _memoryConnectionMatrix[_memoryConnectionMatrix.Count-1][i] = memoryModuleIndex;
                memoryModuleIndex++;
            }

            // 4. Левая сторона (снизу вверх, без первого и последнего)
            for (int i = _memoryConnectionMatrix.Count - 2; i > 0; i--)
            {
                _memoryConnectionMatrix[i][0] = memoryModuleIndex;
                memoryModuleIndex++;
            }
            //PrintConnectionMatrix();
        }

    }
}
