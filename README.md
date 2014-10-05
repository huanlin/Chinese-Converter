Chinese-Converter
=================

A tool for converting text files between Traditional Chinese and Simplified Chinese.

##使用方法

    ChineseConverter <InputFile> <OutputFile> <ConversionDirection> [DictionaryFile]

參數:

        InputFile           - 輸入檔名。
        OutputFile          - 輸出檔名。
        ConversionDirection - "t2s" 或 "s2t"，分別表示「繁->簡」或「簡->繁」。
        DictionaryFile      - 字典檔。每一行代表一個以 '=' 號分隔的簡繁術語對應，例如：「物件導向=面向對象」。 


範例：

    ChineseConverter test.txt test.chs.txt t2s cht2chs.dict

