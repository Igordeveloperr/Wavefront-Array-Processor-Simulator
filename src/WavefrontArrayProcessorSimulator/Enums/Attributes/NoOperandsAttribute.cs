using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVS.enums.attribute
{
    // Специальный атрибут для указания, что операция не принимает операндов
    [AttributeUsage(AttributeTargets.Field)]
    internal class NoOperandsAttribute : Attribute
    {
    }
}
