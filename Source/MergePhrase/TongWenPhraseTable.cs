using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MergePhrase
{
    public class TongWenPhraseTable
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }

        public Dictionary<string, string> Map { get; set; }
    }
}
