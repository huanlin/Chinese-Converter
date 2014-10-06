Chinese Converter
=================

此工具是用來轉換繁體中文與簡體中文檔案。基本上，由繁入簡或由簡至繁應該都可以，但目前只測試過繁至簡的轉換。

由於此工具使用了 MS Word 來進行簡繁轉換，故你的 Windows 作業環境必須有安裝 MS Word。
我只在 Word 2013 上測試過。若您發現任何問題，歡迎回報 bugs。

註：此文件的簡體中文版 README.chs.md 就是利用此工具產生的。

###使用方法

    ChineseConverter <InputFile> <OutputFile> <ConversionDirection> [DictionaryFile]

**參數:**

 * InputFile: 輸入檔名。目前僅支援 UTF-8 編碼。
 * OutputFile: 輸出檔名。目前僅支援 UTF-8 編碼。若檔案已存在，將會被覆蓋。
 * ConversionDirection: "t2s" 或 "s2t"，分別表示「繁->簡」或「簡->繁」。
 * DictionaryFile: 字典檔。每一行代表一個以 '=' 號分隔的簡繁術語對應，例如：「物件導向=面向對象」。註：此字典檔係用於前置轉換作業，亦即輸入檔案會先經過此字典檔的轉換，然後才餵給 MS Word  進行簡繁／繁簡轉換。


**範例：**

    ChineseConverter README.md README.chs.md t2s cht2chs.dict

###原始碼 

完整原始碼：<https://github.com/huanlin/Chinese-Converter>

其中的 Dictionary 資料夾是用來存放自定義的簡繁術語字典。請注意，這些字典檔案只是用來補足 MS Word 未提供的術語，而不是完整的術語對照表。
我打算在這裡維護一份自己使用的字典，每次碰到缺的詞彙就加進去。如果您也有用這個工具，歡迎協助添加字典檔。

注意：添加詞彙至字典檔時，如果其他字典檔已經存在該詞彙，則程式在執行轉換時，只會取「第一個」詞彙對照，而忽略其餘重複定義的詞彙。

###未來可能加入的功能

 * 支援多個自訂字典檔，以便將字典分類並個別維護。如此便可為不同領域提供不同的字典（例如資訊技術、法律、生物等），並且在執行轉換時視需要結合多個字典檔。
