using NLog;
using System.Collections.Generic;
using WaveVS.PE;
using WaveVS.WaveFrontVS.PE.command;

namespace WaveVS.WaveFrontVS.PE.program
{
    internal class PEProgram
    {
        // Создаем логгер для текущего класса
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MnemonicDecoder _mnemonicDecoder;
        private readonly List<MnemonicCommand> _mnemonicProgram;
        private readonly List<string> _codeView;
        private int _ip = 0;
        private readonly IdStructure _idStructure;

        public string CurrentCmd { get; private set; }
        public IdStructure CurrentId { get; set; }
        public int? Ip => _ip == _mnemonicProgram.Count ? (int?)null : _ip;
        public List<string> CodeView { get { return _codeView; } }

        public PEProgram(
            ProcessorElement processor
        )
        {
            _mnemonicProgram = new List<MnemonicCommand>();
            _mnemonicDecoder = new MnemonicDecoder(
                processor
            );
            _idStructure = processor.Id;
            _codeView = new List<string>();
        }

        // Сброс указателя команд.
        public void Reset()
        {
            _ip = 0;
            CurrentCmd = "";
        }

        // формируем программу на уровне мнемоник
        public void Load(List<MnemonicCommand> instructions)
        {
            Reset();
            _codeView.Clear();
            _mnemonicProgram.Clear();
            foreach (var instruction in instructions)
            {
                _mnemonicProgram.Add(instruction);
                _codeView.Add(instruction.Address + " " + instruction);
            }
        }
    
        // выполнение программы
        public void Execute()
        {
            while (_ip < _mnemonicProgram.Count)
            {
                Step();
            }
            if (CurrentId.I == _idStructure.I && CurrentId.J == _idStructure.J)
            {
                Logger.Info("-----------------------------------------------------");
            }
        }

        public void Step()
        {
            if (_ip >= _mnemonicProgram.Count)
                return;

            var instruction = _mnemonicProgram[_ip];
            var command = _mnemonicDecoder.DecodeOperationMnemonic(instruction.Operation);
            CurrentCmd = instruction.ToString();
            command.Execute(_mnemonicDecoder.DecodeOperands(instruction));
            _ip++;

            if (CurrentId.I == _idStructure.I && CurrentId.J == _idStructure.J)
                Logger.Info($"ПЭ{_idStructure.I}{_idStructure.J}: {CurrentCmd}");
        }
    }
}
