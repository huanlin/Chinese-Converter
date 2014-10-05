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
    /// 修改自「黑暗執行緒」的文章：http://blog.darkthread.net/post-2013-08-15-office-tcscconverter.aspx
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

        public string Convert(string src, TSChineseConverterDirection direction)
        {
            string result = null;
            var tcscDirection = WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC;
            if (direction == TSChineseConverterDirection.TraditionalToSimplified)
            {
                tcscDirection = WdTCSCConverterDirection.wdTCSCConverterDirectionTCSC;
            }
            doc.Content.Text = src;
            doc.Content.TCSCConverter(tcscDirection, true, true);
            //取回轉換結果時，結尾會多出\r
            result = doc.Content.Text.TrimEnd('\r');
            return result;
        }

        /// <summary>
        /// 轉換整個檔案。
        /// </summary>
        /// <param name="srcFileName">來源檔名。</param>
        /// <param name="dstFileName">目的檔名。</param>
        /// <param name="direction">轉換方向。</param>
        /// <param name="customDictionary">自訂詞彙轉換表。注意：在呼叫 WORD 轉換之前會先使用此表進行字串替換。</param>
        public void Convert(string srcFileName, string dstFileName, TSChineseConverterDirection direction, 
            Dictionary<string, string> customDictionary = null)
        {
            if (!File.Exists(srcFileName))
            {
                throw new FileNotFoundException("指定的來源檔案不存在: " + srcFileName);
            }

            using (var dstFileWriter = new StreamWriter(dstFileName))
            {
                using (var srcFileReader = new StreamReader(srcFileName))
                {
                    string line = srcFileReader.ReadLine();
                    while (line != null)
                    {
                        if (customDictionary != null)
                        {
                            line = ApplyCustomDictionary(line, customDictionary);
                        }

                        string convertedLine = Convert(line, direction);
                        dstFileWriter.WriteLine(convertedLine);

                        line = srcFileReader.ReadLine();
                    }
                    srcFileReader.Close();
                    dstFileWriter.Close();
                }
            }            
        }

        private string ApplyCustomDictionary(string input, Dictionary<string, string> customDictionary)
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (string key in customDictionary.Keys)
            {
                sb.Replace(key, customDictionary[key]);
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            //確實關閉Word Application
            try
            {
                //關閉Word檔
                object dontSave = WdSaveOptions.wdDoNotSaveChanges;
                ((_Document)doc).Close(ref dontSave);
                //確保Document COM+釋放
                if (doc != null)
                    Marshal.FinalReleaseComObject(doc);
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
