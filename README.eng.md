Chinese Converter
=================

A tool for converting text files between Traditional Chinese and Simplified Chinese.

MS Word is required to run this tool. I've only tested with Word 2013. Feel free to report bugs if you encounter any problems.

##Usage

    ChineseConverter <InputFile> <OutputFile> <ConversionDirection> [DictionaryFile]

**Arguments:**

 * InputFile: Input file name. UTF-8 encoding only.
 * OutputFile: Output file name. If the file exists, it will be overwritten. UTF-8 encoding only.
 * ConversionDirection: "t2s" or "s2t" represnets "Traditional to Simplified" or "Simplified to Traditional".
 * DictionaryFile: Custom dictionary file. Each line represents a mapping of one Chinese word/phrase, separated with '=' character. For example: "物件導向=面向對象". Note: this file will be used as pre-processor before invoking MS Word conversion.


**範例：**

    ChineseConverter test.txt test.chs.txt t2s cht2chs.dict

