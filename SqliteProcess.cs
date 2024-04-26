using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json.Linq;
using Kelimecim.DataAccess;
using Kelimecim.Models;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace Kelimecim
{
    public class SqliteProcess
    {
        private Random rn = new Random();

        List<Vocabulary> wordMeaningDBBeginner = new List<Vocabulary>();
        List<Vocabulary> wordMeaningDBIntermediate = new List<Vocabulary>();
        List<Vocabulary> wordMeaningDBTotal = new List<Vocabulary>();
        List<Sentences> sentences = new List<Sentences>();

        List<Vocabulary> userWordsMeaning = new List<Vocabulary>();
        //List<string> sentencesWithWords = new List<string>();
        //List<string> sentencesWords = new List<string>();
        //List<string> sentencesMeanings = new List<string>();

        private static readonly object lockObject = new object();
        private static SqliteProcess _instance;
        private readonly KelimecimDbContext vocabularyDb;

        public SqliteProcess(KelimecimDbContext vocabularyDb)
        {
            this.vocabularyDb = vocabularyDb;
            try
            {
                TablolarıGetir(vocabularyDb);
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                        MauiProgram.InitializeDatabase();  // Yeniden başlat
                }
            }
        }

        private void TablolarıGetir(KelimecimDbContext vocabularyDb)
        {
            wordMeaningDBBeginner.AddRange(from item in vocabularyDb.EnglishTurkishDictionaryA1ToB2
                                           select new Vocabulary { Word = item.Word, Meaning = item.Meaning });
            wordMeaningDBIntermediate.AddRange(from item in vocabularyDb.EnglishTurkishDictionaryB2ToC1
                                               select new Vocabulary { Word = item.Word, Meaning = item.Meaning });

            userWordsMeaning.AddRange(from item in vocabularyDb.UsersDictionary
                                      select new Vocabulary { Word = item.Word, Meaning = item.Meaning });

            sentences.AddRange(from item in vocabularyDb.MixedSentencesInEnglish
                               select new Sentences { Word = item.Word, Meaning = item.Meaning, Sentence = item.Sentence });

            wordMeaningDBTotal.AddRange(wordMeaningDBBeginner);
            wordMeaningDBTotal.AddRange(wordMeaningDBIntermediate);
        }

        // Public static Instance property
        public static SqliteProcess Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)  // Ensure thread safety
                    {
                        if (_instance == null)
                        {
                            // Bu satırda DbContext oluşturmanız gerekebilir ya da dışarıdan almanız gerekir
                            // Ancak singleton pattern genellikle parametresiz bir constructor gerektirir.
                            _instance = new SqliteProcess(new KelimecimDbContext()); // Parametre gerektirebilir
                        }
                    }
                }
                return _instance;
            }
        }
        //public bool kelimeSayfasiHazirMi = false;
        //public bool CumleSayfasiHazirMi = false;
        //public bool CoktanSecmeliSayfasiHazirMi = false;


        //private static SqliteProcess _instance;

        public string KelimeAraTR(string kelime)
        {
            string trKelime = kelime.ToLower();

            string meaningOfWord = wordMeaningDBTotal
                .Where(vocab => string.Equals(vocab.Meaning, kelime, StringComparison.OrdinalIgnoreCase))
                .Select(vocab => vocab.Word)
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
        public string KelimeAraENG(string word)
        {
            string enKelime = word.ToLower();

            string wordmeaning = wordMeaningDBTotal
                .Where(vocab => string.Equals(vocab.Word, word, StringComparison.OrdinalIgnoreCase))
                .Select(vocab => vocab.Meaning)
                .FirstOrDefault();
            if (wordmeaning != null)
            {
                return wordmeaning;
            }
            else
            {
                //Web'de çeviri yapma özelliği ekleyecem.
                return "Kelime bulunamadı";
            }
        }


        /// <summary>
        /// Rastgele kelime ve anlamını çekmek için oluşturduğum method. Sözlükten veri çekilmesini istiyorsanız değişken true olmalı. Veri ekleyerek oluşturduğumuz listeden veri çekmek için false olması gerekiyor.
        /// </summary>
        /// <param name="VeritabaniMi"></param>
        /// <returns></returns>
        public Tuple<string, string> RastgeleKelimeGetirVTOrMyList(bool VeritabaniMi)
        {
            //if (!VeritabaniMi)
            //    Sayfa1VeriGuncelle();

            //kaç satırlık veri var bunların değerini çekiyorum.
            int sonsatirMyList = userWordsMeaning.Count();
            int sonsatirVT = wordMeaningDBTotal.Count();// şimdilik hepsi

            int hangiSatirMyList = rn.Next(0, sonsatirMyList);//veri sayısına göre rastgele bir satırdan veri çekmeyi istiyorum.
            int hangiSatirVT = rn.Next(0, sonsatirVT);

            string kelime = VeritabaniMi ? wordMeaningDBTotal[hangiSatirVT].Meaning : userWordsMeaning[hangiSatirMyList].Meaning;//Eğer veritabaniMi sorgusu true gelirse kendi sözlüğümden veri çekip değişkene atayacağım değilse sayfa1'de bulunan kendi eklediğim kelimelerden veri çekip değişkene atayacağım.

            string word = VeritabaniMi ? wordMeaningDBTotal[hangiSatirVT].Word : userWordsMeaning[hangiSatirMyList].Word;//aynı mantıkla kelimenin anlamını çekiyorum.

            return Tuple.Create(KelimeDuzelt(kelime), KelimeDuzelt(word));//verileri Tuple nesnesine çevirip gönderiyorum.
        }

        public string[] Rastgele4KelimeYaDaWordGetir(string dogruKelime, bool tersCevir)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {
            //geliştirme yapmak istersem kullanıcıdan bir tane daha değer isterim boolean olacak şekilde ve true olursa columnKelimeVeri'yi false olursa columnWordData'yı
            List<string> veriler = new List<string>();
            foreach (var i in wordMeaningDBTotal)
            {
                if (tersCevir)
                {
                    veriler.Add(i.Word);
                }
                else
                    veriler.Add(i.Meaning);
            }

            int sonsatirVT = veriler.Count();
            int eklenenIndex = 0;
            string[] kelimeler = new string[4];

            while (eklenenIndex < 4)
            {
                int hangiSatirVTT = rn.Next(0, sonsatirVT);
                string gelenVeri = KelimeDuzelt(veriler[hangiSatirVTT]);
                if (!kelimeler.Contains(gelenVeri) && gelenVeri != KelimeDuzelt(dogruKelime))//Eğer veri yoksa ve doğru kelime değilse eklesin.
                {
                    kelimeler[eklenenIndex] = KelimeDuzelt(gelenVeri);
                    eklenenIndex++;
                }
            }

            return kelimeler;
        }
        public string[] Rastgele4KelimeYaDaWordGetir(string dogruKelime)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {

            List<string> veriSeti = wordMeaningDBTotal.Select(x => x.Meaning).ToList();

            int sonsatirVT = veriSeti.Count();
            int eklenenIndex = 0;
            string[] kelimeler = new string[4];

            while (eklenenIndex < 4)
            {
                int hangiSatirVTT = rn.Next(0, sonsatirVT);
                string gelenVeri = KelimeDuzelt(veriSeti[hangiSatirVTT]);
                if (!kelimeler.Contains(gelenVeri) && gelenVeri != KelimeDuzelt(dogruKelime))
                {
                    kelimeler[eklenenIndex] = gelenVeri;
                    eklenenIndex++;
                }
            }
            return kelimeler;
        }
        public Tuple<string, string, string> RastgeleCumle()//Cümleleri alınca koyacam.
        {
            int sonSatir = sentences.Count();
            Random rn = new Random();
            int hangiSayi = rn.Next(0, sonSatir);

            string cumle = sentences.Select(x => x.Sentence).ToList()[hangiSayi];
            string word = sentences.Select(x => x.Word).ToList()[hangiSayi];
            string kelime = sentences.Select(x => x.Meaning).ToList()[hangiSayi];

            return Tuple.Create(cumle, word, kelime);
        }
        private bool VeriSorgu(string word)//dB'DE VAR MI YOK MU
        {
            //Sayfa1VeriGuncelle();

            bool varMi = userWordsMeaning.Select(x => x.Word).ToList().Any(x => x.Equals(word, StringComparison.OrdinalIgnoreCase));

            return varMi;
        }
        public List<Vocabulary> SearchWords(string query)
        {
            return vocabularyDb.UsersDictionary
                .Select(w => new Vocabulary { Word = w.Word, Meaning = w.Meaning })
                .Where(w => w.Word.ToLower().StartsWith(query.ToLower()))
                .ToList();
        }
        public int UserTablosundaKacVeriVar()//dB'DE VAR MI YOK MU
        {

            return vocabularyDb.UsersDictionary.ToList().Count(); ;
        }
        public void VeriEkle(string kelime, string anlam)
        {

            if (VeriSorgu(kelime))
                return;
            else
            {
                userWordsMeaning.Add(new Vocabulary { Word = kelime, Meaning = anlam });
                vocabularyDb.UsersDictionary.Add(new UsersDictionary { Word = kelime, Meaning = anlam });
                vocabularyDb.SaveChanges();
            }
        }
        public bool VeriSil(string word)
        {
            bool silindiMi = false;
            if (userWordsMeaning.Select(x => word).ToList().Any(x => x.Equals(word, StringComparison.OrdinalIgnoreCase)))
            {
                userWordsMeaning.Remove(userWordsMeaning.FirstOrDefault(x => x.Word == word));
                vocabularyDb.UsersDictionary.Remove(vocabularyDb.UsersDictionary.FirstOrDefault(x => x.Word == word));
                vocabularyDb.SaveChanges();
                silindiMi = true;
            }
            return silindiMi;
        }

        public Tuple<List<string>, List<string>> Sayfa1Veri()
        {
            //Sayfa1VeriGuncelle();
            List<string> ASutunu = userWordsMeaning.Select(x => x.Word).ToList();
            List<string> BSutunu = userWordsMeaning.Select(x => x.Meaning).ToList();

            ASutunu.Reverse();
            BSutunu.Reverse();
            Tuple<List<string>, List<string>> veri = new Tuple<List<string>, List<string>>(ASutunu, BSutunu);

            return veri;
        }

        public string Ceviri(string text, bool tr)
        {
            using (HttpClient client = new HttpClient())
            {
                string ceviridil = tr ? "tr|en" : "en|tr";
                string apiUrl = $"https://api.mymemory.translated.net/get?q={text}&langpair={ceviridil}";

                HttpResponseMessage response = client.GetAsync(apiUrl).Result; // Bekleyerek sonucu al

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result; // Bekleyerek içeriği al
                    dynamic json = JObject.Parse(responseBody);
                    string translatedText = json.responseData.translatedText;

                    return translatedText;
                }
                else
                {
                    return "Çevirisiz.";
                }
            }
        }


        private string KelimeDuzelt(string kelime)//Sayfama veri eklerken bu formatta eklensin istiyorum.
        {
            string sonuc = char.ToUpper(kelime[0]) + kelime.Substring(1).ToLower();
            return sonuc.Trim();
        }
    }

    public class Vocabulary
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
    }
    public class Sentences
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string Sentence { get; set; }
    }
    class VeriYonlendir
    {
        private static VeriYonlendir _instance;

        public static VeriYonlendir Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VeriYonlendir();
                }
                return _instance;
            }
        }
        public List<string> gosterilenKelimelerYanlis { get; set; }
        public List<string> gosterilenKelimelerDogru { get; set; }
        public bool trMii { get; set; }

        // Yapıcı metod
        public VeriYonlendir()
        {
            // Listeleri başlat
            gosterilenKelimelerYanlis = new List<string>();
            gosterilenKelimelerDogru = new List<string>();
        }
    }
}
