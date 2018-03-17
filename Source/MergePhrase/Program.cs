using System;
using System.IO;
using System.Text;
using ChineseConverter;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;

namespace MergePhrase
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts));
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            // Setup DI
            var serviceProvider = new ServiceCollection() // Microsoft.Extensions.DependencyInjection
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .BuildServiceProvider();

            // Create logger
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(opts.LogFileName, Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            Log.Information("MergePhrase 應用程式開始執行。");

            var dict = new TSChineseDictionary(Log.Logger);
            if (File.Exists(opts.OutputFileName))
            {
                dict.Load(opts.OutputFileName);
            }

            dict.OverwriteExistingPhrase = opts.OverwriteExistingPhrase;

            foreach (var fname in opts.InputFileNames)
            {
                if (!File.Exists(fname))
                {
                    Console.WriteLine($"檔案不存在：{fname}");
                    continue;
                }
                Console.WriteLine($"正在處理 {fname} ...");
                if (fname.EndsWith(".json"))
                {
                    ParseTongWenJsonFile(fname, dict);
                }
                else
                {
                    ParseTextFile(fname, dict);
                }
            }
            dict.Save(opts.OutputFileName);

            Log.Information("MergePhrase 應用程式執行完畢。");
        }

        private static void ParseTongWenJsonFile(string fname, TSChineseDictionary dict)
        {
            var content = File.ReadAllText(fname, Encoding.UTF8);
            var tongWenTable = JsonConvert.DeserializeObject<TongWenPhraseTable>(content);
            int lastProgress = -1;
            dict.Add(tongWenTable.Map, 
                (sender, args) =>
                {
                    if (lastProgress != args.ProgressPercentage)
                    {
                        lastProgress = args.ProgressPercentage;
                        if (lastProgress % 10 == 0)
                        {
                            Console.WriteLine($"{lastProgress}%");
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }                    
                });
        }

        private static void ParseTextFile(string fname, TSChineseDictionary dict)
        {
            using (var reader = new StreamReader(fname, Encoding.UTF8))
            {
                int lineCount = 0;
                string s = reader.ReadLine();
                while (s != null)
                {
                    if (!s.StartsWith(';'))
                    {
                        string[] words = s.Split('=', ',');
                        if (words.Length >= 2)
                        {
                            dict.Add(words[0], words[1]);
                        }
                    }
                    lineCount++;
                    if (lineCount % 100 == 0)
                    {
                        Console.Write(".");
                    }                    
                    s = reader.ReadLine();
                }
            }
        }
    }
}
