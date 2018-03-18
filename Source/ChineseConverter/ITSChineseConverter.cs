namespace ChineseConverter
{
    public interface ITSChineseConverter
    {
        string Convert(string input, TSChineseConverterDirection direction, 
            TSChineseDictionary customDictionary = null);
        void Convert(string inFileName, string outFileName, TSChineseConverterDirection direction,
            TSChineseDictionary customDictionary = null);

        string ToSimplifiedChinese(string input, TSChineseDictionary customDictionary = null);
        void ToSimplifiedChinese(string inFileName, string outFileName, TSChineseDictionary customDictionary = null);
        string ToTraditionalChinese(string input, TSChineseDictionary customDictionary = null);
        void ToTraditionalChinese(string inFileName, string outFileName, TSChineseDictionary customDictionary = null);
    }
}
