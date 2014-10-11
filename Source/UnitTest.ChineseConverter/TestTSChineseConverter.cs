using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ChineseConverter;

namespace UnitTest.ChineseConverter
{
    [TestClass]
    public class TestTSChineseConverter
    {
        [TestMethod]
        public void TestConvert()
        {
            var myDict = new TSChineseDictionary();
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
    }
}
