using System;
using System.Collections.Generic;
using WaveVS.Services.dto;

namespace WaveVS.Services
{
    internal static class NumberSystemsService
    {
        public static BinNumber ConvertDecToBin(float? hex)
        {
            float convData = hex == null ? 0 : hex.Value;
            byte[] bytes = BitConverter.GetBytes(convData);
            uint intBits = BitConverter.ToUInt32(bytes, 0);
            string binary = Convert.ToString(intBits, 2).PadLeft(32, '0');
            return new BinNumber(
                $"{binary[0]}",
                $"{binary.Substring(1, 8)}",
                $"{binary.Substring(9)}"
            );
        }

        public static float ConvertBinToDec(BinNumber binNumber)
        {
            string fullBinary = binNumber.Sign + binNumber.Exponent + binNumber.Mantis;
            uint intBits = Convert.ToUInt32(fullBinary, 2);
            byte[] bytes = BitConverter.GetBytes(intBits);
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
