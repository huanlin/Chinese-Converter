using ChineseConverter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace UnitTest.ChineseConverter
{
    [TestClass]
    public class TestTSChineseConverter
    {
        [ClassInitialize]
        public static void InitBeforeAllTests(TestContext testContext)
        {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.Debug()
                    .CreateLogger();
        }

        [TestMethod]
        public void ConvertToSimplifiedChinese()
        {
            var myDict = new TSChineseDictionary(Log.Logger);
            myDict.Add("擴充方法=擴展方法;extension method")
                  .Add("預設=默認=default")
                  .Add("建構函式=構造函數")
                  .Add("類別名稱=類名")
                  .Add("類別=類;class");          

            string input = "Convert() 是一個擴充方法，它所擴充的類別名稱是 Foo。Foo 類別有提供預設建構函式。";
            string expectedResult = "Convert() 是一个扩展方法，它所扩充的类名是 Foo。Foo 类有提供默认构造函数。";
            var converter = new TSChineseConverter();
            string result = converter.ToSimplifiedChinese(input, myDict);
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void ConvertToSimplifiedChineseWithNestedMapping()
        {
            // 測試巢狀定義。所謂的巢狀定義指的是：一筆對應的來源字串包含於另一筆對應的目標字串。
            var myDict = new TSChineseDictionary(Log.Logger);
            myDict.Load("NestedMappingTest.dict");

            Assert.AreEqual(myDict.HasError, true); // 應該要能偵測到字典檔裡面有巢狀定義的情形。

            string input = "從應用程式組態檔中讀取欲使用的類別名稱";
            string expectedResult = "从应用程序配置文件中读取欲使用的类名";
            var converter = new TSChineseConverter();
            string result = converter.ToSimplifiedChinese(input, myDict);
            Assert.AreEqual(result, expectedResult);                
        }
    }
}
