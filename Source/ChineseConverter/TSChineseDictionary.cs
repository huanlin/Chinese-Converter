using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;

namespace ChineseConverter
{
    /// <summary>
    /// Traditional & Simplified Chinese dictionary (mapping table).
    /// 
    /// <para>此類別封裝了簡繁中文詞彙的對應表。對應表的內容可以從檔案載入，也可以儲存至指定的檔案名稱。載入對應表之後，便可將它餵給 TSChineseConverter 的 Convert 方法。<br/>
    /// 簡繁詞彙對應表是個純文字檔，每一行代表一個術語的簡繁對應，可以是由繁至簡，亦可由簡至繁。格式為「來源詞彙=目的詞彙」。
    /// 比如說，你要進行繁->簡轉換，那麼你可能會編寫一個名叫 cht2chs.txt 的字典檔，內容如下：</para>
    /// 
    /// <code>
    /// 相依性注入=依赖注入
    /// 擴充方法=扩展方法
    /// 預設=默认
    /// 建構函式=构造函数
    /// </code>
    /// 
    /// <para>建立此對應表時必須注意避免因為「來源詞彙」太短或一詞多義而造成錯誤的詞彙轉換。例如「擴充方法=扩展方法」不要簡化為「擴充=扩展」。</para>
    /// </summary>
    public class TSChineseDictionary
    {
        // 在搜尋取代字串時，要先處理長度比較長的詞彙，故使用此類別來將字串長度越長的詞彙排在越前面處理。
        // 例如簡體中文的「一出戲」（一齣戲）和「一出戲院」。
        private SortedDictionary<string, string> _dictionary;
        private ILogger _logger;

        public bool OverwriteExistingPhrase { get; set; } = false;
        public bool HasError { get; private set; } = false;

        public TSChineseDictionary(ILogger logger)
        {
            var cmp = new WordMappingLengthComparer();
            _dictionary = new SortedDictionary<string, string>(cmp);
            _logger = logger;
        }

        public void Reset()
        {
            HasError = false;
        }

        public void Load(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    Add(line);
                    line = reader.ReadLine();
                }
            }            
        }

        public void Load(string[] fileNames)
        {
            foreach (string fname in fileNames)
            {
                Load(fname);
            }
        }

        public void Save(string fileName)
        {
            var sorted = SortByCharacter();
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                foreach (var key in sorted.Keys)
                {
                    writer.WriteLine($"{key}={sorted[key]}");
                }
            }
        }

        private SortedDictionary<string, string> SortByCharacter() 
            => new SortedDictionary<string, string>(_dictionary);

        public TSChineseDictionary Add(string sourceWord, string targetWord)
        {
            foreach (string value in _dictionary.Values) 
            {
                if (value == sourceWord) // 檢查是否循環定義
                {
                    // 如果來源字串是先前定義過的目標字串，則忽略。否則會出現後面的定義蓋掉先前的定義。
                    string s = $"來源字串【{sourceWord}】 已出現在先前定義的目標字串中, 故忽略此項。";
                    _logger.Warning(s);
                    HasError = true;
                    return this;
                }
            }

            if (_dictionary.ContainsKey(sourceWord))
            {
                if (OverwriteExistingPhrase)
                {
                    string orgTargetWord = _dictionary[sourceWord];
                    _dictionary[sourceWord] = targetWord;
                    _logger.Information($"【{sourceWord}={orgTargetWord}】 的目標字串被新的 【{targetWord}】 取代。");
                }
                else
                {
                    _logger.Warning($"【{sourceWord}={targetWord}】 的來源字串重複定義, 故忽略此項。");
                }
            }
            else
            {
                _dictionary.Add(sourceWord, targetWord);
            }           
            return this;
        }

        public TSChineseDictionary Add(string mapping)
        {
            return Add(mapping, '=');
        }

        public TSChineseDictionary Add(string mapping, char separator)
        {
            if (!String.IsNullOrWhiteSpace(mapping) && !mapping.StartsWith(";"))
            {
                int separatorIndex = mapping.IndexOf(separator);
                if (separatorIndex > 0)
                {
                    string sourceWord = mapping.Substring(0, separatorIndex);
                    string targetWord = mapping.Substring(separatorIndex + 1);
                    // 移除後面的註解。註解可以是分號(';')或等號('=') 
                    char[] commentSeparators = new char[] { ';', separator };
                    int commentIndex = targetWord.IndexOfAny(commentSeparators);
                    if (commentIndex > 0)
                    {
                        targetWord = targetWord.Substring(0, commentIndex);
                    }
                    Add(sourceWord, targetWord);
                }
            }
            return this;
        }

        public TSChineseDictionary Add(IDictionary<string, string> dict, EventHandler<ProgressEventArgs> onProgress = null)
        {
            int total = dict.Count;
            int count = 0;
            foreach (var key in dict.Keys)
            {
                string value = dict[key];
                Add(key, value);
                count++;
                int progress = (100 * count) / total;
                onProgress?.Invoke(this, new ProgressEventArgs(progress, $"{key}={value}"));
            }
            return this;
        }

        public string Convert(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (var key in _dictionary.Keys)
            {
                sb.Replace(key, _dictionary[key]);
            }
            return sb.ToString();
        }

        public void DumpKeys()
        {
            foreach (var key in _dictionary.Keys)
            {
                Console.WriteLine(key);
            }
        }
    }

    /// <summary>
    /// 依片語的長度來比序：越長的字串排在越前面；字串長度相等者，則採用字串預設的比序。
    /// </summary>
    internal class WordMappingLengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.Length == y.Length)
            {
                return x.CompareTo(y);
            }
            return y.Length - x.Length;
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int percentage, string message)
        {
            ProgressPercentage = percentage;
            Message = message;
        }

        public int ProgressPercentage { get; }
        public string Message { get; }
    }
}
