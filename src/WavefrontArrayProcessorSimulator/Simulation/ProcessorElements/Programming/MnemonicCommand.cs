using System;
using System.Collections.Generic;
using WaveVS.enums;

namespace WaveVS.WaveFrontVS.PE.command
{
    internal class MnemonicCommand
    {
        // поля команды на уровне мнемоник
        public string Address { get; }
        public OperationEnum Operation { get; }
        public RegisterOperandEnum RegisterOperand1 { get; }
        public BufferOperandEnum BufferOperand1 { get; }
        public RegisterOperandEnum RegisterOperand2 { get; }
        public BufferOperandEnum BufferOperand2 { get; }

        // команда типа буфер
        public bool IsNoOperands { get; } = false;
        // команда типа буфер
        public bool IsBuff { get; } = false;
        // команда типа регистр
        public bool IsReg { get; } = false;
        // команда типа регистр - регистр
        public bool IsRegReg { get; } = false;
        // команда типа регистр - буфер
        public bool IsRegBuff { get; } = false;
        // команда типа буфер - регистр
        public bool IsBuffReg { get; } = false;
        // команда типа буфер - буфер
        public bool IsBuffBuff { get; } = false;

        // нет операндов
        public MnemonicCommand(
            string address,
            OperationEnum operation
        )
        {
            Address = address;
            Operation = operation;
            IsNoOperands = true;
        }

        // один операнд - буфер
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            BufferOperandEnum bufferOperand1
        )
        {
            Address = address;
            Operation = operation;
            BufferOperand1 = bufferOperand1;
            IsBuff = true;
        }

        // один операнд - регистр
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            RegisterOperandEnum registerOperand1
        )
        {
            Address = address;
            Operation = operation;
            RegisterOperand1 = registerOperand1;
            IsReg = true;
        }

        // регистр - регистр
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            RegisterOperandEnum registerOperand1,
            RegisterOperandEnum registerOperand2
        )
        {
            Address = address;
            Operation = operation;
            RegisterOperand1 = registerOperand1;
            RegisterOperand2 = registerOperand2;
            IsRegReg = true;
        }

        // регистр - буфер
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            RegisterOperandEnum registerOperand1,
            BufferOperandEnum bufferOperand2
        )
        {
            Address = address;
            Operation = operation;
            RegisterOperand1 = registerOperand1;
            BufferOperand2 = bufferOperand2;
            IsRegBuff = true;
        }

        // буфер - регистр
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            BufferOperandEnum bufferOperand1,
            RegisterOperandEnum registerOperand2
        )
        {
            Address = address;
            Operation = operation;
            RegisterOperand2 = registerOperand2;
            BufferOperand1 = bufferOperand1;
            IsBuffReg = true;
        }

        // буфер - буфер
        public MnemonicCommand(
            string address,
            OperationEnum operation,
            BufferOperandEnum bufferOperand1,
            BufferOperandEnum bufferOperand2
        )
        {
            Address = address;
            Operation = operation;
            BufferOperand1 = bufferOperand1;
            BufferOperand2 = bufferOperand2;
            IsBuffBuff = true;
        }
        public override string ToString()
        {
            if (IsNoOperands) return Operation.ToString();
            if (IsReg) return $"{Operation} {RegisterOperand1}";
            if (IsBuff) return $"{Operation} {BufferOperand1}";
            if (IsRegReg) return $"{Operation} {RegisterOperand1}, {RegisterOperand2}";
            if (IsRegBuff) return $"{Operation} {RegisterOperand1}, {BufferOperand2}";
            if (IsBuffReg) return $"{Operation} {BufferOperand1}, {RegisterOperand2}";
            return $"{Operation} {BufferOperand1}, {BufferOperand2}";
        }
    }
}
