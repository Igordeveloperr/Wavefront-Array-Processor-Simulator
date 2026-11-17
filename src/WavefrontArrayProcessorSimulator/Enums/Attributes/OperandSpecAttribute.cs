using System;

namespace WaveVS.enums.attribute
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class OperandSpecAttribute : Attribute
    {
        public int Position { get; }
        public Type OperandType1 { get; }
        public Type OperandType2 { get; }

        public OperandSpecAttribute(int position, Type operandType1)
        {
            Position = position;
            OperandType1 = operandType1;
            OperandType2 = null;
        }

        public OperandSpecAttribute(int position, Type operandType1, Type operandType2)
        {
            Position = position;
            OperandType1 = operandType1;
            OperandType2 = operandType2;
        }

        // Свойство для получения всех типов операндов
        public Type[] GetAllOperandTypes()
        {
            if (OperandType2 != null)
            {
                return new Type[] { OperandType1, OperandType2 };
            }
            return new Type[] { OperandType1 };
        }
    }
}