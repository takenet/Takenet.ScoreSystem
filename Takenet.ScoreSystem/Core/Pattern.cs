using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.ScoreSystem.Core
{
    public class Pattern
    {
        public Pattern(string signature, double value, byte historySize)
        {
            Signature = signature;
            Value = value;
            HistorySize = historySize;
        }
        public string Signature { get; set; }
        public double Value { get; set; }
        public byte HistorySize { get; set; }
    }
}
