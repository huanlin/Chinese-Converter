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
    /// 簡繁術語對應表是個純文字檔，每一行代表一個術語的簡繁對應，可以是由繁入簡，亦可由簡至繁。格式為「來源詞彙=目的詞彙」。
    /// 比如說，你要進行繁->簡轉換，那麼你可能會編寫一個名叫 cht2chs.dict 的字典檔，內容如下：</para>
    /// 
    /// <code>
    /// 相依性注入=依賴注入
    /// 擴充方法=擴展方法
    /// 預設=默認
    /// 建構函式=構造函數
    /// 類別名稱=類名
    /// 類別=類型
    /// </code>
    /// 
    /// <para>請注意上述範例雖然用於繁->簡轉換，但是等號（'='）右邊的「目的詞彙」並不需要使用簡體字。
    /// 這是因為此對應表會在 MS Word 轉換之前就先行套用；就如剛才提到的，此對應表是作用於 Ms Word 簡繁轉換之前的前置作業。</para>
    /// 
    /// <para>建立此對應表時必須注意避免因為「來源詞彙」太短或一詞多義而造成錯誤的詞彙轉換。例如「擴充方法=擴展方法」不可簡化為「擴充=擴展」。</para>
    /// </summary>
    public class TSChineseDictionary
    {
        // 在搜尋取代字串時，要先處理長度比較長的詞彙，故使用此類別來將字串長度越長的詞彙排在越前面處理。
        // 例如：「類別名稱」
        private SortedDictionary<string, string> _dictionary;

        public TSChineseDictionary()
        {
            var cmp = new WordMappingComparer();
            _dictionary = new SortedDictionary<string, string>(cmp);
        }

        public void Load(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!String.IsNullOrWhiteSpace(line) && !line.StartsWith(";") && line.IndexOf('=') > 0)
                    {
                        var words = line.Split('=');
                        if (words.Length == 2)
                        {
                            this.Add(words[0], words[1]);
                        }
                    }
                    line = reader.ReadLine();
                }
            }            
        }

        public TSChineseDictionary Add(string sourceWord, string targetWord)
        {
            // Skip duplicated words.
            if (!_dictionary.ContainsKey(sourceWord))
            {
                _dictionary.Add(sourceWord, targetWord);
            }
            return this;
        }

        public TSChineseDictionary Add(string mapping)
        {
            if (!String.IsNullOrWhiteSpace(mapping) && !mapping.StartsWith(";") && mapping.IndexOf('=') > 0)
            {
                var words = mapping.Split('=');
                if (words.Length == 2)
                {
                    this.Add(words[0], words[1]);
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
