﻿using System;
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

        public Pattern(Core.Pattern pattern) : base("pattern", Guid.NewGuid().ToString())
        {
            Signature = pattern.Signature;
            Value = pattern.Value;
            MaxHistorySize = pattern.MaxHistorySize;
            MinHistorySize = pattern.MinHistorySize;
            Description = pattern.Description;
        }

        public string Signature { get; set; }
        public double Value { get; set; }
        public int MinHistorySize { get; set; }
        public int MaxHistorySize { get; set; }
        public string Description { get; set; }

        public static implicit operator Core.Pattern(Pattern pattern)
        {
            return new Core.Pattern(pattern.Signature, pattern.Value, (byte)pattern.MinHistorySize, (byte)pattern.MaxHistorySize);
        }
        public static implicit operator Pattern(Core.Pattern pattern)
        {
            return new Pattern(pattern);
        }

    }
}
