﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ChineseConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup DI
            var serviceProvider = new ServiceCollection() // Microsoft.Extensions.DependencyInjection
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .BuildServiceProvider();

            // Create logger
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("d:/0/1.log", Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            Log.Information("MergePhrase 應用程式開始執行。");

            Console.WriteLine("ChineseConverter v0.3 by Huan-Lin Tsai (2022)");

            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            string srcFileName = args[0];
            string dstFileName = args[1];

            string direction = args[2];
            string dirLabel = null;
            var convDirection = TSChineseConverterDirection.SimplifiedToTraditional;
            if ("t2s".Equals(direction, StringComparison.InvariantCultureIgnoreCase))
            {
                convDirection = TSChineseConverterDirection.TraditionalToSimplified;
                dirLabel = "繁->簡";
            }
            else if ("s2t".Equals(direction, StringComparison.InvariantCultureIgnoreCase)) 
            {
                convDirection = TSChineseConverterDirection.SimplifiedToTraditional;
                dirLabel = "簡->繁";
            }
            else 
            {
                Console.WriteLine("Invalid conversion direction: " + direction);
                Console.WriteLine("Use 't2s' or 's2t'.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("輸入檔案: {0}", srcFileName);
            Console.WriteLine("輸出檔案: {0}", dstFileName);
            Console.WriteLine("轉換操作: {0}", dirLabel);

            TSChineseDictionary dict = null;
            if (args.Length >= 4)
            {
                dict = new TSChineseDictionary(Log.Logger);
                int index = 3;
                while (index < args.Length)
                {
                    string dictFileName = args[index];
                    if (!File.Exists(dictFileName))
                    {
                        Console.WriteLine("檔案不存在: " + dictFileName);
                        return;
                    }                    
                    dict.Load(dictFileName);
                    Console.WriteLine("使用字典: {0}", dictFileName);

                    if (dict.HasError)
                    {
                        Console.WriteLine("處理過程中發現錯誤或值得注意的事項，請閱讀 log 檔案以了解細節。");
                        Console.WriteLine();
                    }
                    dict.Reset();

                    index++;
                }
            }                

            // 執行轉換
            Console.WriteLine("\r\n正在轉換.....");

            var converter = new TSChineseConverter();
            try 
            {
                converter.Convert(srcFileName, dstFileName, convDirection, dict);
                Console.WriteLine();
                Console.WriteLine("轉換完畢!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("轉換時發生錯誤: ");
                Console.WriteLine(ex.Message);
            }
        }

        static Dictionary<string, string> LoadDictionary(string dictFileName)
        {
            var dict = new Dictionary<string, string>();

            using (var reader = new StreamReader(dictFileName))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!String.IsNullOrWhiteSpace(line) && !line.StartsWith(";") && line.IndexOf('=') > 0)
                    {
                        var words = line.Split('=');
                        if (words.Length == 2)
                        {
                            dict.Add(words[0], words[1]);
                        }
                    }
                    line = reader.ReadLine();
                }
            }

            return dict;
        }

        static void ShowUsage() 
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("ChineseConverter <InputFile> <OutputFile> <ConversionDirection> [Dictionary File(s)]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("ConversionDirection - t2s or s2t.");
        }
    }
}
