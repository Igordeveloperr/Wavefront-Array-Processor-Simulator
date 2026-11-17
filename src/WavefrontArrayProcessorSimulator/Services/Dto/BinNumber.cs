using System;
using System.Collections.Generic;

namespace WaveVS.Services.dto
{
    internal struct BinNumber
    {
        public string Sign;
        public string Exponent;
        public string Mantis;

        public BinNumber(string sign, string exponent, string mantis)
        {
            Sign = sign;
            Exponent = exponent;
            Mantis = mantis;
        }
    }
}
