using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace MergePhrase
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "輸入檔案名稱，至少一個。")]
        public IEnumerable<string> InputFileNames { get; set; }

        [Option('o', "output", Required = true, HelpText = "輸出檔案名稱。")]
        public string OutputFileName { get; set; }

        [Option('l', "log", Required = false, Default = "MergePhrase.log", HelpText = "Log 檔案名稱。")]
        public string LogFileName { get; set; }

        [Option('w', "overwrite", Required = false, Default = false, HelpText = "覆蓋既有的字詞。")]
        public bool OverwriteExistingPhrase { get; set; }
       
        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "顯示詳細的處理過程。")]
        public bool Verbose { get; set; }
    }
}
