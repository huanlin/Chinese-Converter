using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace ChineseConverter
{
    public enum TSChineseConverterDirection
    {
        TraditionalToSimplified,
        SimplifiedToTraditional
    };

    /// <summary>
    /// Traditional & Simplified Chinese converter.
    /// 
    /// 初版原始碼參考了「黑暗執行緒」的文章：http://blog.darkthread.net/post-2013-08-15-office-tcscconverter.aspx
    /// 感謝黑大!
    /// </summary>
    public class TSChineseConverter : IDisposable
    {
        private Application wordApp = null;
        private Document doc = null;

        public TSChineseConverter()
        {
            wordApp = new Application();
            wordApp.Visible = false;
            doc = wordApp.Documents.Add();
        }

        private string MSWordConvertChinese(string input, TSChineseConverterDirection direction)
        {
            string result = null;
            var tcscDirection = WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC;
            if (direction == TSChineseConverterDirection.TraditionalToSimplified)
            {
                tcscDirection = WdTCSCConverterDirection.wdTCSCConverterDirectionTCSC;
            }
            doc.Content.Text = input;
            doc.Content.TCSCConverter(tcscDirection, true, true);
            // 去除轉換之後字串結尾多出來的 \r 字元
            result = doc.Content.Text.TrimEnd('\r');
            return result;
        }      

        public string Convert(string input, TSChineseConverterDirection direction, TSChineseDictionary customDictionary = null)
        {
            // Pre-conversion using custom dictionary.
            if (customDictionary != null)
            {
                input = customDictionary.Convert(input);
            }

            // Conversion using Microsoft Word.
            string result = MSWordConvertChinese(input, direction);
            return result;
        }

        /// <summary>
        /// 轉換整個檔案。
        /// </summary>
        /// <param name="inFileName">來源檔名。目前僅支援 UTF-8 編碼。</param>
        /// <param name="outFileName">目的檔名。目前僅支援 UTF-8 編碼。</param>
        /// <param name="direction">轉換方向。</param>
        /// <param name="customDictionary">自訂詞彙轉換表。注意：在呼叫 WORD 轉換之前會先使用此表進行字串替換。詳細用法參見 TSChineseDictionary 類別的說明。</param>
        public void Convert(string inFileName, string outFileName, TSChineseConverterDirection direction, 
            TSChineseDictionary customDictionary = null)
        {
            if (!File.Exists(inFileName))
            {
                throw new FileNotFoundException("指定的來源檔案不存在: " + inFileName);
            }

            using (var dstFileWriter = new StreamWriter(outFileName))
            {
                using (var srcFileReader = new StreamReader(inFileName))
                {
                    string line = srcFileReader.ReadLine();
                    while (line != null)
                    {
                        string convertedLine = this.Convert(line, direction, customDictionary);
                        dstFileWriter.WriteLine(convertedLine);
                        line = srcFileReader.ReadLine();
                    }
                    srcFileReader.Close();
                    dstFileWriter.Close();
                }
            }            
        }

        #region Shortcut methods
        public string ToSimplifiedChinese(string input, TSChineseDictionary customDictionary = null)
        {
            string result = this.Convert(input, TSChineseConverterDirection.TraditionalToSimplified, customDictionary);
            return result;
        }

        public void ToSimplifiedChinese(string inFileName, string outFileName, TSChineseDictionary customDictionary = null)
        {
            this.Convert(inFileName, outFileName, TSChineseConverterDirection.TraditionalToSimplified, customDictionary);
        }

        public string ToTraditionalChinese(string input, TSChineseDictionary customDictionary = null)
        {
            string result = this.Convert(input, TSChineseConverterDirection.SimplifiedToTraditional, customDictionary);
            return result;
        }

        public void ToTraditionalChinese(string inFileName, string outFileName, TSChineseDictionary customDictionary = null)
        {
            this.Convert(inFileName, outFileName, TSChineseConverterDirection.SimplifiedToTraditional, customDictionary);
        }

        #endregion Shortcut methods

        public void Dispose()
        {
            // 確實關閉 Word Application
            try
            {
                // 關閉 Word 檔
                object dontSave = WdSaveOptions.wdDoNotSaveChanges;
                ((_Document)doc).Close(ref dontSave);
                // 確保 Document COM 物件 釋放
                if (doc != null) 
                {
                    Marshal.FinalReleaseComObject(doc);
                }
                    
                doc = null;
                ((_Application)wordApp).Quit(ref dontSave);
            }
            finally
            {
                Marshal.FinalReleaseComObject(wordApp);
            }
        }
    }
}
