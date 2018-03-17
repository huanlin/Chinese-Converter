# MergePhrase

## 簡介

設計此工具的目的是為了重複利用網路上既有的中文簡繁對應詞彙，並將這些詞彙納入 ChineseConverter 的詞彙庫。

工具可用來將下列來源的簡繁對應詞庫（片語、字典）檔案結合成一個新的檔案：

- [TongWeb.Core](https://github.com/tongwentang/tongwen-core) 的字典檔，例如 s2t_phrase.json。
- [EPUBConverter-Go](https://github.com/Swind/EPUBConverter-Go) 的字典檔，例如 phrase_s2t.txt 和 word_s2t.txt。
- [Meihua-Chinese-Converter](https://github.com/shyangs/Meihua-Chinese-Converter)，需要手動修改成純文字格式（見下方說明），然後再利用此工具合併。

此工具所產生的新檔案，是採用 ChineseConverter 的格式。參考以下關於檔案格式的說明：

    ;=====================================
    ;格式：來源詞彙=目的詞彙
    ;範例：專案=項目
    ;分號後面的文字都會被忽略（不處理）。
    ;=====================================

## 用法

dotnet MergePhrase.dll -i <輸入檔案1> <輸入檔案2> -o <輸出檔案> -l <Log 檔案> [options]

options:

 -w : 碰到已經有的字詞，就用新的（來源）覆蓋掉現有的。若沒有指定此選項，則會保留既有詞彙（即碰到重複的字詞就忽略）。

來源檔案可以有一個或多個。此工具會按照命令列指定的順序來依序將每一個來源檔案合併至目的檔案。

請注意：

- 檔案的字元編碼都必須是 UTF-8。
- 來源檔案的格式，是以副檔名來判斷。如果副檔名是 .json，則會被視為 TongWen.Core 的字典檔，若是 .txt，則會視為以逗號分隔來源與目的詞彙的純文字檔案。底下是兩種格式的範例。

### TongWen.Core 字典範例

~~~~~~~~
{
  "name": "預設簡化轉傳統 - 詞彙",
  "filename": "s2t_phrase",
  "type": "s2t",
  "enabled": true,
  "map": {
    "一出": "一出",
    "一出京劇": "一齣京劇",
    "一出劇": "一齣劇",
    "一出劇院": "一出劇院",
    "一出好戲": "一齣好戲",
    "一制藥": "一製藥"
  }
}
~~~~~~~~

### 純文字的字典範例

~~~~~~~~
一出京劇,一齣京劇
一出劇院,一出劇院
一出好戲,一齣好戲
~~~~~~~~

## 相依套件

- [Command Line Parser Library for CLR and NetStandard](https://github.com/commandlineparser/commandline)
- [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
- [Serilog](https://serilog.net/)
