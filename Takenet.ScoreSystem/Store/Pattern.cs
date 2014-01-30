using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Takenet.ScoreSystem.Store
{
    public class Pattern : TableEntity
    {

        public Pattern()
        {
            
        }

        public Pattern(Core.Pattern pattern) : base("pattern", pattern.Signature)
        {
            Value = pattern.Value;
            HistorySize = pattern.HistorySize;
        }

        public string Signature { get { return this.RowKey; }  }
        public double Value { get; set; }
        public int HistorySize { get; set; }


        public static implicit operator Core.Pattern(Pattern pattern)
        {
            return new Core.Pattern(pattern.Signature, pattern.Value, (byte)pattern.HistorySize);
        }
        public static implicit operator Pattern(Core.Pattern pattern)
        {
            return new Pattern(pattern);
        }

    }
}
