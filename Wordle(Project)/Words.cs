using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace Wordle_Project_
{
    //Gets a random word from a list of words. Done like this so you can get the words from a file or a web service.
    public static class WordService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string wordListUrl = "https://raw.githubusercontent.com/tabatkins/wordle-list/main/words";
        private static List<string> wordList;

        public static async Task<string> GetRandomWord()
        {
            if (wordList == null)
            {
                await LoadWordList();
            }

            var random = new Random();
            return wordList[random.Next(wordList.Count)].ToUpper();
        }

        public static async Task<bool> IsValidWord(string word)
        {
            if (wordList == null)
            {
                await LoadWordList();
            }

            return wordList.Contains(word.ToLower());
        }

        private static async Task LoadWordList()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wordlist.txt");

            if (!File.Exists(filePath))
            {
                var words = await client.GetStringAsync(wordListUrl);
                await File.WriteAllTextAsync(filePath, words);
            }

            wordList = (await File.ReadAllLinesAsync(filePath)).ToList();
        }
    }
}
