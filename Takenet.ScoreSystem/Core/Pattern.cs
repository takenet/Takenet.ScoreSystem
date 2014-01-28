using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.ScoreSystem.Core
{
    public class Pattern
    {
        public Pattern(string signature, decimal value)
        {
            Signature = signature;
            Value = value;
        }
        public string Signature { get; set; }
        public decimal Value { get; set; }
    }
}
