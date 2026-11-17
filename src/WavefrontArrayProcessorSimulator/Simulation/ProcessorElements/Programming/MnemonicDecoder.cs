using System;
using System.Collections.Generic;
using WaveVS.enums;
using WaveVS.Memory;
using WaveVS.PE;
using WaveVS.WaveFrontVS.PE.command;
using WaveVS.WaveFrontVS.PE.program.commandPattern;

namespace WaveVS.WaveFrontVS.PE.program
{
    internal class MnemonicDecoder
    {
        // словарь позволяющий из мнемоники регистра получить соответствующий регистр
        private readonly Dictionary<RegisterOperandEnum, Register> _registerOperandDecoder;
        // словарь позволяющий из мнемоники входного буфера получить соответствующий входной буфер
        private readonly Dictionary<BufferOperandEnum, InputBuffer> _bufferOperandDecoder;
        // словарь позволяющий из мнемоники операции получить соответствующую операцию
        private readonly Dictionary<OperationEnum, Command> _operationDecoder;

        public MnemonicDecoder(
            ProcessorElement processor
        )
        {
            // заполняем декодеры мнемоник
            _registerOperandDecoder = new Dictionary<RegisterOperandEnum, Register>()
            {
                { RegisterOperandEnum.R_RES, processor.ResultRegister},
                { RegisterOperandEnum.R0, processor.RegisterFile[0]},
                { RegisterOperandEnum.R1, processor.RegisterFile[1]},
                { RegisterOperandEnum.R2, processor.RegisterFile[2]},
                { RegisterOperandEnum.R3, processor.RegisterFile[3]},
                { RegisterOperandEnum.R4, processor.RegisterFile[4]},
                { RegisterOperandEnum.R5, processor.RegisterFile[5]},
                { RegisterOperandEnum.R6, processor.RegisterFile[6]},
                { RegisterOperandEnum.R7, processor.RegisterFile[7]},
            };
            _bufferOperandDecoder = new Dictionary<BufferOperandEnum, InputBuffer>() 
            {
                { BufferOperandEnum.L_BF, processor.LeftInputBuffer },
                { BufferOperandEnum.T_BF, processor.TopInputBuffer },
            };
            _operationDecoder = new Dictionary<OperationEnum, Command>()
            {
                { OperationEnum.MOV, new MovCommand() },
                { OperationEnum.ADD, new AddCommand() },
                { OperationEnum.SUB, new SubCommand() },
                { OperationEnum.MUL, new MulCommand() },
                { OperationEnum.DIV, new DivCommand() },
                { OperationEnum.WRITE_BUFF, new WriteBuffersCommand(processor) },
                { OperationEnum.SEND, new SendCommand(processor) },
                { OperationEnum.SEND_R, new SendRightCommand(processor) },
                { OperationEnum.SEND_D, new SendDownCommand(processor) },
            };
        }

        // получение объекта команды по мнемонике
        public Command DecodeOperationMnemonic(OperationEnum mnemonic)
        {
            return _operationDecoder[mnemonic];
        }

        // получение регистра по мнемонике
        public Register DecodeRegisterMnemonic(RegisterOperandEnum mnemonic)
        {
            return _registerOperandDecoder[mnemonic];
        }

        // получение буфера по мнемонике
        public InputBuffer DecodeBufferMnemonic(BufferOperandEnum mnemonic)
        {
            return _bufferOperandDecoder[mnemonic];
        }

        public object[] DecodeOperands(MnemonicCommand instruction)
        {
            if (instruction.IsNoOperands) return new object[0];
            if (instruction.IsReg) return new object[] { DecodeRegisterMnemonic(instruction.RegisterOperand1) };
            if (instruction.IsBuff) return new object[] { DecodeBufferMnemonic(instruction.BufferOperand1) };
            if (instruction.IsRegReg) return new object[] { DecodeRegisterMnemonic(instruction.RegisterOperand1), DecodeRegisterMnemonic(instruction.RegisterOperand2) };
            if (instruction.IsRegBuff) return new object[] { DecodeRegisterMnemonic(instruction.RegisterOperand1), DecodeBufferMnemonic(instruction.BufferOperand2) };
            if (instruction.IsBuffReg) return new object[] { DecodeBufferMnemonic(instruction.BufferOperand1), DecodeRegisterMnemonic(instruction.RegisterOperand2) };
            return new object[] { DecodeBufferMnemonic(instruction.BufferOperand1), DecodeBufferMnemonic(instruction.BufferOperand2) };
        }

        // декодирование адреса строки
        public int DecodeLineAddress(string address)
        {
            return int.Parse(
                address.Substring(2), 
                System.Globalization.NumberStyles.HexNumber);
        }
    }
}
