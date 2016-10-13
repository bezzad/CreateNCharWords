using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace CreateAllNCharWords
{
    class Program
    {
        public const int MaxArrayLength = 1000000; // 1 Million

        public static readonly string[] EnglishChars =
        {
            "a", "b", "c", "d",
            "e", "f", "g", "h",
            "i", "j", "k", "l",
            "m", "n", "o", "p",
            "q", "r", "s", "t",
            "u", "v", "w", "x",
            "y", "z"
        };

        private static string ResultSavePath { get; set; }

        static void Main(string[] args)
        {
            int len;

            Start:

            Console.Clear();
            Console.WriteLine("What length of words you want to generate?");
            var lenStr = Console.ReadLine();

            if (int.TryParse(lenStr, out len))
            {
                ResultSavePath = @"C:\ResultOf_{length}_CharWords.txt";
                Console.Clear();
                Console.WriteLine("Please choose result path:");
                Console.WriteLine(ResultSavePath);
                var p = Console.ReadLine();
                ResultSavePath = string.IsNullOrEmpty(p) ? ResultSavePath.Replace("{length}", lenStr) : p.Replace("{length}", lenStr);

                File.WriteAllText(ResultSavePath, "");
                Console.WriteLine("Starting...");
                Thread.Sleep(1000);
                Console.Clear();

                var allWordsCount = (long)Math.Pow(EnglishChars.Length, len);
                var printIndexer = allWordsCount / Math.Min(allWordsCount, 100);
                var result = new string[Math.Min((int)allWordsCount, MaxArrayLength)];

                long counter = 0;
                long cutterLastIndex = MaxArrayLength; // cut array to disk if larger that limitation

                var engCharIndexBuffer = new Int16[len];
                for (long i = 0; i < allWordsCount; i++)
                {
                    result[counter++ % MaxArrayLength] = WriteWordByIndex(engCharIndexBuffer);

                    GotoNextWord(ref engCharIndexBuffer);

                    if (counter >= cutterLastIndex)
                    {
                        SaveToDisk(ref result);
                        cutterLastIndex += MaxArrayLength;
                    }

                    if (counter % printIndexer == 0)
                    {
                        Console.Clear();
                        Console.WriteLine($"{counter} / {allWordsCount} Completed.  [{counter * 100 / allWordsCount}%]");
                    }
                }

                Console.Clear();
                Console.WriteLine($"{allWordsCount} / {allWordsCount} Completed.");
                SaveToDisk(ref result);
                Console.WriteLine("Process Completed Successful.");
                Console.WriteLine($"Result Path: {ResultSavePath}");
            }
            else
            {
                Console.WriteLine("Your entry length is incorrect!");
                Console.WriteLine("Please press any key to try again.");
                Console.ReadKey();
                goto Start;
            }

            Console.ReadKey();
            goto Start;
        }

        private static void SaveToDisk(ref string[] result)
        {
            File.AppendAllLines(ResultSavePath, result);
            GC.Collect(GC.MaxGeneration);
        }

        private static void GotoNextWord(ref Int16[] engCharIndexBuffer)
        {
            for (var i = engCharIndexBuffer.Length - 1; i >= 0; i--)
            {
                if (engCharIndexBuffer[i] >= EnglishChars.Length - 1)
                    engCharIndexBuffer[i] = 0;
                else
                {
                    engCharIndexBuffer[i]++;
                    break;
                }
            }
        }

        private static string WriteWordByIndex(Int16[] engCharIndexBuffer)
        {
            return engCharIndexBuffer.Aggregate("", (current, s) => current + EnglishChars[s]);
        }
    }
}
