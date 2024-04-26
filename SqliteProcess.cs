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
        List<Vocabulary> wordMeaningDBBeginner = new List<Vocabulary>();
        List<Vocabulary> wordMeaningDBIntermediate = new List<Vocabulary>();

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

            //using(SqliteConnection connection = new SqliteConnection("Data Source=Db\\KelimecimDb.db"))
            //{
            //    connection.Open();
            //    SqliteCommand cmd = new SqliteCommand("Select word, meaning from EnglishTurkishDictionaryA1ToB2",connection);
            //    SqliteDataReader reader = cmd.ExecuteReader();
            //    while(reader.Read())
            //    {
            //        wordsDB.Add(reader["word"].ToString());
            //        meaningsDB.Add(reader["meaning"].ToString());
            //    }

            //    SqliteCommand cmd2 = new SqliteCommand("Select word, meaning from UserDB", connection);
            //    SqliteDataReader reader2 = cmd.ExecuteReader();
            //    while (reader2.Read())
            //    {
            //        userWords.Add(reader2["word"].ToString());
            //        userMeanings.Add(reader2["meaning"].ToString());
            //    }
            //}
        }
        public bool kelimeSayfasiHazirMi = false;
        public bool CumleSayfasiHazirMi = false;
        public bool CoktanSecmeliSayfasiHazirMi = false;


        private static SqliteProcess _instance;

        private Random rn = new Random();

        //public static SqliteProcess Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new SqliteProcess();
        //        }
        //        return _instance;
        //    }
        //}
    



       
}
    public class Vocabulary
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
   
    }
}
