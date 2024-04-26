using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json.Linq;
using Kelimecim.DataAccess;
using Kelimecim.Models;
using System.Security.Cryptography.X509Certificates;


namespace Kelimecim
{
    public class SqliteProcess
    {
        private Random rn = new Random();

        List<Vocabulary> wordMeaningDBBeginner = new List<Vocabulary>();
        List<Vocabulary> wordMeaningDBIntermediate = new List<Vocabulary>();
        List<Vocabulary> wordMeaningDBTotal = new List<Vocabulary>();

        List<Vocabulary> userWordsMeaning = new List<Vocabulary>();
        //List<string> sentencesWithWords = new List<string>();
        //List<string> sentencesWords = new List<string>();
        //List<string> sentencesMeanings = new List<string>();

        private readonly KelimecimDbContext vocabularyDb;
        public SqliteProcess(KelimecimDbContext vocabularyDb)
        {
            this.vocabularyDb = vocabularyDb;
            foreach (var item in vocabularyDb.EnglishTurkishDictionaryA1ToB2S)
            {
                wordMeaningDBBeginner.Add(new Vocabulary { Word = item.Word, Meaning = item.Meaning });//A1-B2 arası kelimeler
            }
            foreach (var item in vocabularyDb.EnglishTurkishDictionaryB2ToC1S)
            {
                wordMeaningDBIntermediate.Add(new Vocabulary { Word = item.Word, Meaning = item.Meaning });//B2-C1 arası kelimeler
            }
            foreach (var item in vocabularyDb.UsersDictionaries)
            {
                userWordsMeaning.Add(new Vocabulary { Word = item.Word, Meaning = item.Meaning });//Kullanıcının database'inden veri çekmek için
            }

            wordMeaningDBTotal.AddRange(wordMeaningDBBeginner);
            wordMeaningDBTotal.AddRange(wordMeaningDBIntermediate);

        }
        //public bool kelimeSayfasiHazirMi = false;
        //public bool CumleSayfasiHazirMi = false;
        //public bool CoktanSecmeliSayfasiHazirMi = false;


        //private static SqliteProcess _instance;

        public string KelimeAraTR(string kelime)
        {
            string trKelime = kelime.ToLower();

            string meaningOfWord = wordMeaningDBTotal
                .Where(vocab => string.Equals(vocab.Word, kelime, StringComparison.OrdinalIgnoreCase))
                .Select(vocab => vocab.Meaning)
                .FirstOrDefault();
            if (meaningOfWord != null)
            {
                return meaningOfWord;
            }
            else
            {
                //Web'de çeviri yapma özelliği ekleyecem.
                return "Kelime bulunamadı";
            }

        }





    }
    public class Vocabulary
    {
        public string Word { get; set; }
        public string Meaning { get; set; }

    }
}
