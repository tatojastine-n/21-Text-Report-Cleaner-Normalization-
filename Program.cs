using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

public class TextCleaner
{
    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
        "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
        "to", "was", "were", "will", "with"
    };

    public string Process(string text)
    {
        text = text.Trim();

        text = RemoveNonAscii(text);

        text = Regex.Replace(text, @"\s+", " ");

        text = text.ToLower();

        text = TitleCaseSentences(text);

        return text;
    }

    public TextStats Analyze(string cleanText)
    {
        var words = Regex.Split(cleanText, @"\W+")
                        .Where(w => !string.IsNullOrEmpty(w))
                        .ToList();

        var wordFreq = words.Where(w => !StopWords.Contains(w.ToLower()))
                          .GroupBy(w => w.ToLower())
                          .OrderByDescending(g => g.Count())
                          .Take(5)
                          .Select(g => (Word: g.Key, Count: g.Count()))
                          .ToList();

        var sentences = Regex.Split(cleanText, @"(?<=[.!?])\s+")
                           .Where(s => !string.IsNullOrEmpty(s))
                           .Count();

        return new TextStats
        {
            WordCount = words.Count,
            SentenceCount = sentences,
            TopWords = wordFreq
        };
    }

    private string RemoveNonAscii(string text)
    {
        return Regex.Replace(text, @"[^\u0020-\u007E]", "");
    }

    private string TitleCaseSentences(string text)
    {
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");

        var processedSentences = sentences.Select(s =>
        {
            if (string.IsNullOrEmpty(s)) return s;

            return char.ToUpper(s[0]) + s.Substring(1);
        });

        return string.Join(" ", processedSentences);
    }
}

public class TextStats
{
    public int WordCount { get; set; }
    public int SentenceCount { get; set; }
    public List<(string Word, int Count)> TopWords { get; set; }
}

namespace Text_Report_Cleaner__Normalization_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your messy text (Ctrl+Z then Enter when done on Windows):");
            string input = ReadMultilineInput();

            var cleaner = new TextCleaner();
            string cleanText = cleaner.Process(input);
            var stats = cleaner.Analyze(cleanText);

            Console.WriteLine("\nCleaned Text:");
            Console.WriteLine(cleanText);

            Console.WriteLine("\nStatistics:");
            Console.WriteLine($"Total Words: {stats.WordCount}");
            Console.WriteLine($"Total Sentences: {stats.SentenceCount}");
            Console.WriteLine("Top 5 Words (excluding stop words):");
            foreach (var item in stats.TopWords)
            {
                Console.WriteLine($"{item.Word}: {item.Count}");
            }
        }

        private static string ReadMultilineInput()
        {
            var sb = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}
