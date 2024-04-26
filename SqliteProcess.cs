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

            string kelime = VeritabaniMi ? userWordsMeaning[hangiSatirMyList].Meaning : wordMeaningDBTotal[hangiSatirVT].Meaning;//Eğer veritabaniMi sorgusu true gelirse kendi sözlüğümden veri çekip değişkene atayacağım değilse sayfa1'de bulunan kendi eklediğim kelimelerden veri çekip değişkene atayacağım.
            
            string word = VeritabaniMi ? userWordsMeaning[hangiSatirVT].Word : wordMeaningDBTotal[hangiSatirMyList].Word;//aynı mantıkla kelimenin anlamını çekiyorum.

            return Tuple.Create(KelimeDuzelt(kelime), KelimeDuzelt(word));//verileri Tuple nesnesine çevirip gönderiyorum.
        }

        public string[] Rastgele4KelimeYaDaWordGetir(string dogruKelime, bool tersCevir)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {
            //geliştirme yapmak istersem kullanıcıdan bir tane daha değer isterim boolean olacak şekilde ve true olursa columnKelimeVeri'yi false olursa columnWordData'yı
            List<string> veriler = new List<string>();
            foreach(var i in wordMeaningDBTotal)
            {
                if(tersCevir)
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
        private bool VeriSorgu(string word)//dB'DE VAR MI YOK MU
        {
            //Sayfa1VeriGuncelle();

            bool varMi = wordMeaningDBTotal.Select(x => x.Word).ToList().Any(x => x.Equals(word, StringComparison.OrdinalIgnoreCase));

            return varMi;
        }
        public async Task VeriEkle(string kelime, string anlam)
        {
            
            if (VeriSorgu(kelime))
                return;
            else
            {
                userWordsMeaning.Add(new Vocabulary { Word = kelime, Meaning = anlam });
                await vocabularyDb.UsersDictionaries.AddAsync(new UsersDictionary { Word = kelime, Meaning = anlam });
                await vocabularyDb.SaveChangesAsync();
            }
        }
        public bool VeriSil(string word)
        {
            bool silindiMi = false;
            if(userWordsMeaning.Select(x=>word).ToList().Any(x=>x.Equals(word, StringComparison.OrdinalIgnoreCase)))
            {
                userWordsMeaning.Remove(userWordsMeaning.FirstOrDefault(x => x.Word == word));
                vocabularyDb.UsersDictionaries.Remove(vocabularyDb.UsersDictionaries.FirstOrDefault(x => x.Word == word));
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
