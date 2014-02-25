using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.ScoreSystem.Core
{
    public class Pattern
    {
        public Pattern(string signature, double value, byte minHistorySize, byte maxHistorySize)
        {
            Signature = signature;
            Value = value;
            MaxHistorySize = maxHistorySize;
            MinHistorySize = minHistorySize;
        }
        public string Signature { get; set; }
        public double Value { get; set; }
        public byte MinHistorySize { get; set; }
        public byte MaxHistorySize { get; set; }
    }
}
