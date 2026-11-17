using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveVS.enums.attribute;

namespace WaveVS.enums
{
    internal enum OperationEnum
    {
        //[OperandSpec(0, typeof(LineAddressOperandEnum))]
        //JZ,

        [OperandSpec(0, typeof(RegisterOperandEnum))]
        [OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        MOV,

        [OperandSpec(0, typeof(RegisterOperandEnum))]
        [OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        ADD,

        [OperandSpec(0, typeof(RegisterOperandEnum))]
        [OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        SUB,

        [OperandSpec(0, typeof(RegisterOperandEnum))]
        [OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        MUL,

        [OperandSpec(0, typeof(RegisterOperandEnum))]
        [OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        DIV,

        //[OperandSpec(0, typeof(RegisterOperandEnum))]
        //[OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        //AND,

        //[OperandSpec(0, typeof(RegisterOperandEnum))]
        //[OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        //OR,

        //[OperandSpec(0, typeof(RegisterOperandEnum))]
        //[OperandSpec(1, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        //XOR,

        [OperandSpec(0, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        SEND_R,

        [OperandSpec(0, typeof(RegisterOperandEnum), typeof(BufferOperandEnum))]
        SEND_D,

        [NoOperands]
        SEND,

        [NoOperands]
        WRITE_BUFF,
    }
}
