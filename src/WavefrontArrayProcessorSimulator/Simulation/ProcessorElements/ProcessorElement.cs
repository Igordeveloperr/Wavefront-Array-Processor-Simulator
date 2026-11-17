using System.Collections.Generic;
using WaveVS.enums;
using WaveVS.Memory;
using WaveVS.WaveFrontVS.PE.command;
using WaveVS.WaveFrontVS.PE.program;

namespace WaveVS.PE
{
    internal class ProcessorElement
    {
        // возможные соседние ПЭ
        public ProcessorElement RightProcessorElement { get; set; } = null;
        public ProcessorElement LeftProcessorElement { get; set; } = null;
        public ProcessorElement DownProcessorElement { get; set; } = null;
        public ProcessorElement UpProcessorElement { get; set; } = null;

        // возможные БП подключенные к ПЭ
        public MemoryModule RightMemoryModule { get; set; } = null;
        public MemoryModule LeftMemoryModule { get; set; } = null;
        public MemoryModule DownMemoryModule { get; set; } = null;
        public MemoryModule UpMemoryModule { get; set; } = null;

        // кол-во регистров в файле ПЭ
        private const int RegisterFileSize = 8;
        // регистровый файл ПЭ
        public Dictionary<int,Register> RegisterFile { get; private set; }
        // буферные регистры ПЭ
        public InputBuffer TopInputBuffer { get; private set; }
        public InputBuffer LeftInputBuffer { get; private set; }
        // регистр результата
        public Register ResultRegister { get; private set; }
        // общий регистр флага
        public bool FlagRegister { get; set; }
        // ПЭ работает в режиме передатчика сообщий(НЕ ВЫПОЛНЯЕТ ВЫЧИСЛЕНИЯ)
        public bool OnlySendMessages { get; set; } 
        // уникальный ID ПЭ в массиве
        public IdStructure Id { get; }
        public float? RightValue { get; set; } = null;
        public float? DownValue { get; set; } = null;

        // состояние ПЭ
        private PEStateEnum _state;
        public PEStateEnum State => _state;

        private readonly PEProgram _program;
        public PEProgram Program => _program;

        public ProcessorElement(IdStructure id)
        {
            _state = PEStateEnum.STOP;
            Id = id;
            InitPElements();
            FlagRegister = false;
            OnlySendMessages = false;
            _program = new PEProgram(
                this
            );
        }

        // Загрузка программы процессорного элемента.
        public void LoadProgram(List<MnemonicCommand> codeLines)
        {
            _program.Load(codeLines);
        }

        // создаем основные компоненты ПЭ
        private void InitPElements()
        {
            // создаю входные буферы
            TopInputBuffer = new InputBuffer();
            LeftInputBuffer = new InputBuffer();
            /*
             для удобной работы, регистр результата отдельно от РОН,
             хоть он к нему также относится
             */
            ResultRegister = new Register();

            // создаю регистровый файл
            RegisterFile = new Dictionary<int,Register>();
            for (int i = 0; i < RegisterFileSize; i++)
            {
                RegisterFile.Add(i, new Register());
            }
        }

        // Сброс ПЭ
        public void Reset()
        {
            _program.Reset();
            // в начальное состояние ПЭ
            _state = PEStateEnum.STOP;
            // чистим регистр результата
            ResultRegister.WriteData(0);
            // чистим буферы
            TopInputBuffer.Clear();
            LeftInputBuffer.Clear();
            // сбрасываем флаги в исходное сосстояние
            FlagRegister = false;
            OnlySendMessages = false;
            // чистим регистровый файл
            ClearRegisterFile();
        }

        // Очистка регистрового файла
        private void ClearRegisterFile()
        {
            for (int i = 0; i < RegisterFileSize; i++)
            {
                RegisterFile[i].WriteData(0);
            }
        }

        public void ExecuteProgram()
        {
            UpdateInputBuffers();
            switch (_state)
            {
                case PEStateEnum.STOP:
                    if (LeftInputBuffer.Value != null && TopInputBuffer.Value != null && !OnlySendMessages)
                        _state = PEStateEnum.CALC;
                    else if (OnlySendMessages && (LeftInputBuffer.Value != null || TopInputBuffer.Value != null))
                        _state = PEStateEnum.SEND;
                    break;
                case PEStateEnum.CALC:
                    _program.Execute();
                    _state = LeftInputBuffer.Value == null && TopInputBuffer.Value == null && !OnlySendMessages
                        ? PEStateEnum.STOP : PEStateEnum.SEND;
                    break;
                case PEStateEnum.SEND:
                    SendRight(OnlySendMessages ? LeftInputBuffer.Value : RightValue);
                    SendDown(OnlySendMessages ? TopInputBuffer.Value : DownValue);
                    _state = PEStateEnum.STOP;
                    break;
            }
        }

        public void StepExecuteProgram()
        {
            if (_state == PEStateEnum.CALC)
            {
                _program.Step();
            }
        }

        // передача данных правому соседу ПЭ
        private void SendRight(float? data)
        {
            Send(data, RightProcessorElement?.LeftInputBuffer, LeftInputBuffer);
        }

        // передача данных нижнему соседу ПЭ
        private void SendDown(float? data)
        {
            Send(data, DownProcessorElement?.TopInputBuffer, TopInputBuffer);
        }

        private static void Send(float? data, InputBuffer destination, InputBuffer source)
        {
            if (data == null || (destination != null && destination.Value != null))
                return;

            destination?.WriteData(data);
            source.Clear();
        }

        // записываем значения в буферные регистры
        private void UpdateInputBuffers()
        {
            // если верхний буфер пуст и над нами НЕ ПЭ, то есть БП
            if (TopInputBuffer.Value == null && UpProcessorElement == null)
            {
                // если очередь не пустая то записываем
                if (!UpMemoryModule.IsEmpty)
                {
                    TopInputBuffer.WriteData(UpMemoryModule.Dequeue());
                }
            }

            // если левый буфер пуст и слева НЕ ПЭ, то есть БП
            if (LeftInputBuffer.Value == null && LeftProcessorElement == null)
            {
                // если очередь не пустая то записываем
                if (!LeftMemoryModule.IsEmpty)
                {
                    LeftInputBuffer.WriteData(LeftMemoryModule.Dequeue());
                }
            }
        }
    }
}
