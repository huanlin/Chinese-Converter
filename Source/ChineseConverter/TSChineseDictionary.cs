using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseConverter
{
    /// <summary>
    /// Traditional & Simplified Chinese dictionary (mapping table).
    /// 
    /// <para>此類別主要是為了補 MS Word 簡繁轉換之不足，作為執行 MS Word 簡繁轉換程序之前的前置轉換。例如 MS Word 並不會將「預設」轉換成「默認」。<br/>
    /// 像這類 MS Word 遺漏的詞彙，你可以自行編寫簡繁術語對應表，並利用此類別的 Load 方法來載入對應表，然後餵給 TSChineseConverter 的 Convert 方法。<br/>
    /// 簡繁術語對應表是個純文字檔，每一行代表一個術語的簡繁對應，可以是由繁入簡，亦可由簡至繁。格式為「來源詞彙=目的詞彙;原文或註解」。
    /// 比如說，你要進行繁->簡轉換，那麼你可能會編寫一個名叫 cht2chs.dict 的字典檔，內容如下：</para>
    /// 
    /// <code>
    /// 相依性注入=依賴注入;dependency injection
    /// 擴充方法=擴展方法;extension method
    /// 預設=默認=default （分號與等號都可以當作註解的分隔字元）
    /// 建構函式=構造函數=constructor
    /// 類別名稱=類名;class name
    /// 型別=類型;type
    /// </code>
    /// 
    /// <para>請注意上述範例雖然用於繁->簡轉換，但是等號（'='）右邊的「目的詞彙」並不需要使用簡體字。
    /// 這是因為此對應表會在 MS Word 轉換之前就先行套用；就如剛才提到的，此對應表是作用於 MS Word 簡繁轉換之前的前置作業。</para>
    /// 
    /// <para>建立此對應表時必須注意避免因為「來源詞彙」太短或一詞多義而造成錯誤的詞彙轉換。例如「擴充方法=擴展方法」不要簡化為「擴充=擴展」。</para>
    /// </summary>
    public class TSChineseDictionary
    {
        // 在搜尋取代字串時，要先處理長度比較長的詞彙，故使用此類別來將字串長度越長的詞彙排在越前面處理。
        // 例如：「類別名稱」
        private SortedDictionary<string, string> _dictionary;
        private bool _hasError;
        private StringBuilder _logs;

        public TSChineseDictionary()
        {
            var cmp = new WordMappingComparer();
            _dictionary = new SortedDictionary<string, string>(cmp);
            _logs = new StringBuilder();
            _hasError = false;
        }

        public void ClearLogs()
        {
            _logs.Clear();
            _hasError = false;
        }

        public void Load(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    this.Add(line);
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

        public TSChineseDictionary Add(string sourceWord, string targetWord)
        {
            // Skip duplicated words.
            if (_dictionary.ContainsKey(sourceWord))
            {
                _hasError = true;
                _logs.AppendLine(String.Format("警告: '{0}={1}' 的來源字串重複定義, 故忽略此項。", sourceWord, targetWord));                
                return this;
            }


            foreach (string value in _dictionary.Values) 
            {
                if (value.Contains(sourceWord)) 
                {
                    // 如果來源字串是先前定義過的目標字串，則忽略。否則會出現後面的定義蓋掉先前的定義（巢狀替換）。
                    string fmt = "警告: 來源字串 '{0}' 已包含在先前定義的目標字串中 ('{1}'), 故忽略此項。";
                    _logs.AppendLine(String.Format(fmt, sourceWord, value));
                    _hasError = true;
                    return this;
                }
            }

            _dictionary.Add(sourceWord, targetWord);
            return this;
        }

        public TSChineseDictionary Add(string mapping)
        {
            if (!String.IsNullOrWhiteSpace(mapping) && !mapping.StartsWith(";"))
            {
                int separatorIndex = mapping.IndexOf('=');
                if (separatorIndex > 0)
                {
                    string sourceWord = mapping.Substring(0, separatorIndex);
                    string targetWord = mapping.Substring(separatorIndex + 1);
                    // 移除後面的註解，註解可以是分號(';')或等號('=') 
                    char[] commentSeparators = new char[] { ';', '=' };
                    int commentIndex = targetWord.IndexOfAny(commentSeparators);
                    if (commentIndex > 0)
                    {
                        targetWord = targetWord.Substring(0, commentIndex);
                    }
                    this.Add(sourceWord, targetWord);
                }
            }
            return this;
        }

        public TSChineseDictionary Add(IDictionary<string, string> dict)
        {
            foreach (var key in dict.Keys)
            {
                Add(key, dict[key]);
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

        public bool HasError
        {
            get
            {
                return _hasError;
            }
        }

        public string Logs
        {
            get
            {
                return _logs.ToString();
            }
        }
    }

    internal class WordMappingComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            // 越長的字串排在越前面，字串長度相等者，則採用字串預設的比序。
            if (x.Length == y.Length)
            {
                return x.CompareTo(y);
            }
            return y.Length - x.Length;
        }
    }
}
