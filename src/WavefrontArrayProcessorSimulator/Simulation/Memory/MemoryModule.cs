using System;
using System.Collections.Generic;

namespace WaveVS.Memory
{
    internal class MemoryModule
    {
        private int _cellCount = 4;
        private const string NO_FOUND_ADDRESS_ERROR = "Адрес не принадлежит данному модулю памяти!";
        private const string QUEUE_OVERFLOW_ERROR = "Переполнение очереди!";
        private const string QUEUE_UNDERFLOW_ERROR = "Очередь пуста!";

        private Dictionary<int, float> _memory;
        private int _startAddress;
        private int _endAddress;
        private int _head; // Указатель на начало очереди (первый элемент)
        private int _tail; // Указатель на конец очереди (следующий за последним)
        private int _count; // Количество элементов в очереди

        public Dictionary<int, float> Memory => _memory;
        public int StartAddress => _startAddress;

        public int EndAddress => _endAddress;

        public int Head => _head;

        public int Tail => _tail;

        public bool IsEmpty => _count == 0;

        public bool IsFull => _count == _cellCount;

        public int Count => _count;

        public int Id { get; }

        public MemoryModule(int id)
        {
            _startAddress = 0;
            _endAddress = _startAddress + _cellCount - 1;
            _memory = new Dictionary<int, float>();
            _head = _startAddress;
            _tail = _endAddress;
            _count = _cellCount;

            InitMemory();
            Id = id;
        }

        public void SetMemory(Dictionary<int, float> data)
        {
            _memory.Clear();
            for (int i = _startAddress; i <= _endAddress; i++)
            {
                _memory.Add(i, data[i]);
            }
        }

        private void InitMemory()
        {
            for (int i = _startAddress; i <= _endAddress; i++)
            {
                _memory.Add(i, 0);
            }
        }

        public void UpdateMemory(int cellCount)
        {
            int endAddress = _startAddress + cellCount - 1;
            _memory.Clear();
            for (int i = _startAddress; i <= endAddress; i++)
            {
                _memory.Add(i, 0);
            }
            _endAddress = endAddress;
            _head = _startAddress;
            _tail = _endAddress;
            _count = cellCount;
            _cellCount = cellCount;
        }

        // сброс очереди БП
        public void ResetQueue()
        {
            // сбрасываем голову в начало очереди
            _head = _startAddress;
            // пересчитываем кол-во элементов очереди
            _count = _cellCount;
        }

        // Основные операции очереди

        /// <summary>
        /// Добавить значение в конец очереди (Enqueue)
        /// </summary>
        public void Enqueue(float value)
        {
            if (IsFull)
            {
                throw new InvalidOperationException(QUEUE_OVERFLOW_ERROR);
            }

            _memory[_tail] = value;
            _tail = IncrementAddress(_tail);
            _count++;
        }

        /// <summary>
        /// Извлечь значение из начала очереди (Dequeue)
        /// </summary>
        public float? Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException(QUEUE_UNDERFLOW_ERROR);
            }

            float? value = _memory[_head];
            _head = IncrementAddress(_head);
            _count--;
            return value;
        }

        // Вспомогательный метод для циклического увеличения адреса
        private int IncrementAddress(int address)
        {
            if (address == _endAddress)
            {
                return _startAddress;
            }
            return address + 1;
        }

        // Сохранение прямой адресации (если нужно)

        public float ReadCell(int address)
        {
            ValidateAddress(address);
            return _memory[address];
        }

        public void WriteCell(int address, float value)
        {
            ValidateAddress(address);
            _memory[address] = value;
        }

        private void ValidateAddress(int address)
        {
            if (address < _startAddress || address > _endAddress)
                throw new IndexOutOfRangeException(NO_FOUND_ADDRESS_ERROR);
        }
    }
}