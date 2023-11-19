using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Collections;

namespace Kelimecim
{
    public class GoogleSheets
    {
        public bool kelimeSayfasiHazirMi = false;
        public bool CumleSayfasiHazirMi = false;
        public bool CoktanSecmeliSayfasiHazirMi = false;


        private static GoogleSheets _instance;

        private Random rn = new Random(); // Bir methodda bunu tanımlayıp methodu üst üste çağırdığım zaman aynı değer geliyordu. daha geniş bir kapsamda tanımlayınca her türlü farklı cevap vermesi sağlanabiliyormuş.
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        //private static UserCredential credential;
        private static ServiceAccountCredential credential;

        private SheetsService service;

        ValueRange responseSutunA;
        ValueRange responseSutunB;
        ValueRange responseSearchWord;
        ValueRange responseAramaKelime;
        ValueRange responseCumle;
        ValueRange responseCumleWord;
        ValueRange responseCumleKelime;

        IList<IList<object>> sutunAVeri;
        IList<IList<object>> sutunBVeri;
        IList<IList<object>> columnWordData;
        IList<IList<object>> columnKelimeVeri;
        IList<IList<object>> cumleliSayfaCumle;
        IList<IList<object>> cumleliSayfaWord;
        IList<IList<object>> cumleliSayfaKelime;


        string spreadsheetId = "1kafz2KAuvxqSdGbfNOou1S5keIf5wQDIDRLsdm9t6l8"; // excel tablosunun id'si
        string sutun1 = "Sayfa1!A:A"; //hangi satırı yazmak istediğim.
        string sutun2 = "Sayfa1!B:B"; //hangi satırı yazmak istediğim.
        string veriKumesiIng = "Veriler!A:A";
        string veriKumesiTr = "Veriler!B:B";
        string cumlelerCumle = "Cumleler!A:A";
        string cumlelerWord = "Cumleler!B:B";
        string cumlelerKelime = "Cumleler!C:C";
        string range = "Sayfa1!A:B"; //verieklemeSatırı

        SpreadsheetsResource.ValuesResource.AppendRequest verireq;
        ValueRange body;

        //static string myMemoryApiKey = "1fb0d0fab1c449d5df11";
        string appName = "Desktop client 1";
        public static string ClientId = "950495088287-jtk9og97179u21v31u586r6k108j4n7d.apps.googleusercontent.com";
        public static string ClientSecret = "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";

        public static GoogleSheets Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GoogleSheets();
            }
            return _instance;
        }
    }

        //private async Task VerileriCekAsync()
        //{
        //    string json = @"{
        //                ""installed"":{
        //                ""client_id"": ""950495088287-jtk9og97179u21v31u586r6k108j4n7d.apps.googleusercontent.com"",
        //                ""project_id"": ""kelime-defteri-402314"",
        //                ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
        //                ""token_uri"": ""https://oauth2.googleapis.com/token"",
        //                ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
        //                ""client_secret"": ""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""
        //                }}";

        //    using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
        //    {
        //        var authorizationTask = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.FromStream(stream).Secrets,
        //            new[] { SheetsService.Scope.Spreadsheets },
        //            "user",
        //            CancellationToken.None
        //        ).ConfigureAwait(false);

        //        credential = authorizationTask;
        //    }
        //}

        private async Task VerileriCek()
        {
            string json = @"{
                              ""type"": ""service_account"",
                              ""project_id"": ""kelime-defteri-402314"",
                              ""private_key_id"": ""bfed60d6397041b3728e52ef05fbc46abf34d4e0"",
                              ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC92fEwitxPzT2Z\nFJVTsqUdGbswQ9f4rrQwiwdcWlg9FHmiRuHpUFd7Hv75BPVKy+VeBwj6Oz0xSJcy\nbEzkuiUmTPnCqCbxyPBNcq7At8X+xu+GN+XEGf/IslHv9QQrGOc9hA97hLhlfRgc\nhIG/wWwN68AOzDbo5FGNAPGs8e9flnafcuFJ0aQD1SCkT38pr5fKrH8GzRhUxs3K\nvOt+DQJgub+Zwdi0D6PxtAEtCnKS1YZ3jeuRcavc7hadAObmTuvAovFnU39dO/x7\n4UNr2SPOGN/YuWUWzpKHWkMsmDaG50C7XO1iyeVibgijeMMpkABLHnU8vLDk4NLW\nvVYYXLGNAgMBAAECggEAAYKOf8mW836JJyLKBkKlPAxEmB9uBBEVp04vxo0EZyX2\nyrLgIJwuOfE263Gd5tSk1CDfsifO9omihDjmyqntWjbiBKmUN7eWq7MGD0vW5A4e\ngRWIOw1O4sCCVNOAzzvxOOTIP74APnRmgPGP25/U9W5i1mtK44LPYDz57xt0gTDc\nZkGb9X5ES9RJ2YHh88/lHN2xUyqG+eiYlnySSN0MNjAVMJknNMwA3kZfrotWE62l\nq27mblFkm20hEhn+WLpmHq1qSQGNfVFWtkGQY468UfoDPmmcsqNm3gq84FV87Gk1\ngWIr4/P5JesuNURyAcItCACQNPBvs2kIpPaBLEB56QKBgQDysnkkK6xebh31TmxL\noG/8fZEh8vCMxaxAWsUKiVitoRZ9OoPUji9uKYHBpJZIMWF+Wj5tgRrOQeZzb5CN\nOQg70sw7u5GjkYSdzzpw4jo3IFriUMZZHrXNqn9aQzAy6X2uerPK/OKnjINJeoiZ\nH/9K8JPKf6iFPsPOi/Z8XWnHZQKBgQDIQe+wDXWNPgH6ciLScWGIBOJMkkbN5IY7\n3qOj16//LUKRO1gkV2BvmSy+h5Xe/Es4VAlzZcJWJKzHxqOTZfEu2RJG9d/RypKk\nvHpN36Ex0sgSr62+XvBe+0Db3JSz+qlWC0laU06yaJpDLQmRcTnh1gOBHUT4CHgY\nD8SyUGyDCQKBgQDyGdd+nZJ1IKQB8RlW19TequP8WbxcsVQDXojw2dH8YpVsltKr\nVqs52W33HZhMq/X1dVCRLBjxaAvbW493UU1FYCMb8yB1atRAGFjUAtjP5RbEbI9w\nl5IEd/BSunN6VjFpvD1eYKY5PZI52mIpXiHtP9AuUOprARGTGUvpA8ZhgQKBgG5m\ne19RbDb7sleByNS/kQdNufyAv+wOSjqDWS+gXvSM3R/32XXffdjIVzSKxwLxj/5z\nxeoKdYLMITzZs6A1GSu8nCjmsAeWaBXNmpeH6/PtwkMa+uvypw2V8oHDL2+xht1a\nx4u2VbJhnHngQGAgTcrFE5WAr18WPC73snajg88RAoGBAIq/lQlJNi+BmtwPpRyo\nL+0LPVjX2wfXV/TnGwwbAcfki/K1ePn4qYeZHJRmuMXc5XJH1vs0X2jmjTBWMS04\nYq4ubdX9HWZmXbta0rggHAQrlQ3slGqqcb1ngpzETQRZyMHcDVgRh66sLQD1jPMS\nTG3SizdbbLeuqnoK2GTZBr9n\n-----END PRIVATE KEY-----\n"",
                              ""client_email"": ""kelimecimmm@kelime-defteri-402314.iam.gserviceaccount.com"",
                              ""client_id"": ""104340561337007285969"",
                              ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                              ""token_uri"": ""https://oauth2.googleapis.com/token"",
                              ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                              ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/kelimecimmm%40kelime-defteri-402314.iam.gserviceaccount.com"",
                              ""universe_domain"": ""googleapis.com""
                            }";

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                credential = ServiceAccountCredential.FromServiceAccountData(stream);
            }

            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            };
         
                service = new SheetsService(initializer);

            //Api'ye istek atıyorum
            SpreadsheetsResource.ValuesResource.GetRequest request1 =
                service.Spreadsheets.Values.Get(spreadsheetId, sutun1);
            SpreadsheetsResource.ValuesResource.GetRequest request2 =
                service.Spreadsheets.Values.Get(spreadsheetId, sutun2);

            SpreadsheetsResource.ValuesResource.GetRequest reqSearch =
                service.Spreadsheets.Values.Get(spreadsheetId, veriKumesiIng);
            SpreadsheetsResource.ValuesResource.GetRequest reqArama =
                service.Spreadsheets.Values.Get(spreadsheetId, veriKumesiTr);

            SpreadsheetsResource.ValuesResource.GetRequest reqCumleC =
                service.Spreadsheets.Values.Get(spreadsheetId, cumlelerCumle);
            SpreadsheetsResource.ValuesResource.GetRequest reqCumleW =
                service.Spreadsheets.Values.Get(spreadsheetId, cumlelerWord);
            SpreadsheetsResource.ValuesResource.GetRequest reqCumleK =
                service.Spreadsheets.Values.Get(spreadsheetId, cumlelerKelime);

            responseSutunA = await request1.ExecuteAsync();
            responseSutunB = await request2.ExecuteAsync();

            responseSearchWord = await reqSearch.ExecuteAsync();
            responseAramaKelime = await reqArama.ExecuteAsync();

            responseCumle = await reqCumleC.ExecuteAsync();
            responseCumleWord = await reqCumleW.ExecuteAsync();
            responseCumleKelime = await reqCumleK.ExecuteAsync();

            //değerleri listelere çekiyorum.
            sutunAVeri = responseSutunA.Values;
            sutunBVeri = responseSutunB.Values;

            columnWordData = responseSearchWord.Values;
            columnKelimeVeri = responseAramaKelime.Values;
            if (columnWordData.Count > 10)
            {
                kelimeSayfasiHazirMi = true;
                CoktanSecmeliSayfasiHazirMi = true;
            }

            cumleliSayfaCumle = responseCumle.Values;
            cumleliSayfaWord = responseCumleWord.Values;
            cumleliSayfaKelime = responseCumleKelime.Values;
            if (cumleliSayfaCumle.Count > 10)
                CumleSayfasiHazirMi = true;
        }
        private async void InitializeAsync()
        {
            await VerileriCek();//Daha hızlı bir şekilde veri çekmek için işlemleri asenkron olarak gerçekleştirdim ve çalışmasını sağladım.
        }

        public GoogleSheets()
        {
            InitializeAsync();
            //VerileriCekAsync().Wait();
        }

        public Tuple<List<string>, List<string>> KelimeAra(string AranacakWord)
        {
            //Kendi Database'imde bulunan değerleri çekip atıyorum listelere
            List<string> sonucA = new List<string>();
            List<string> sonucB = new List<string>();

            bool kelimeBulundu = false; // Kelimenin bulunup bulunmadığını kontrol etmek için bir bayrak

            if (columnWordData != null && columnWordData.Count > 0)
            {
                for (int i = 0; i < columnWordData.Count; i++)
                {
                    for (int j = 0; j < columnWordData[i].Count; j++)
                    {
                        string[] words = columnWordData[i][j].ToString().Split(new[] { ' ', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string word in words)
                        {
                            if (word.Trim().Equals(AranacakWord, StringComparison.OrdinalIgnoreCase))
                            {
                                sonucA.Add(columnWordData[i][j].ToString());
                                sonucB.Add(columnKelimeVeri[i][j].ToString());
                                kelimeBulundu = true; // Kelimeyi bulduk, bayrağı true yap
                            }
                        }
                    }
                }
            }

            if (!kelimeBulundu)//Eğer database'imde aradığım kelime yok ise api ile çekiyorum veriyi.
            {
                sonucA.Add(KelimeDuzelt(AranacakWord));
                sonucB.Add(Ceviri(AranacakWord, false).ToString());
            }

            var uniqueSonucA = new HashSet<string>(sonucA);
            var uniqueSonucB = new HashSet<string>(sonucB);

            return new Tuple<List<string>, List<string>>(uniqueSonucA.ToList(), uniqueSonucB.ToList());
        }

        /// <summary>
        /// Bu overloading methodu direkt olarak ingilizceden türkçeye arama yapmamız için oluşturduğum bir kod.
        /// tr adlı değişken true olursa devreye girer.
        /// </summary>
        /// <param name="kelime"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public string KelimeAra2(string kelime, bool tr) //Buna gerek yok??
        {
            string result = Ceviri(kelime, true).ToString();

            return result;
        }

        public Tuple<string, string, string> RastgeleCumle()
        {
            int sonSatir = cumleliSayfaCumle.Count();
            Random rn = new Random();
            int hangiSayi = rn.Next(0, sonSatir);

            string cumle = cumleliSayfaCumle[hangiSayi][0].ToString();
            string word = cumleliSayfaWord[hangiSayi][0].ToString();
            string kelime = cumleliSayfaKelime[hangiSayi][0].ToString();

            return Tuple.Create(cumle, word, kelime);
        }


        /// <summary>
        /// Rastgele kelime ve anlamını çekmek için oluşturduğum method. Sözlükten veri çekilmesini istiyorsanız değişken true olmalı. Veri ekleyerek oluşturduğumuz listeden veri çekmek için false olması gerekiyor.
        /// </summary>
        /// <param name="VeritabaniMi"></param>
        /// <returns></returns>
        public Tuple<string, string> RastgeleKelimeGetirVTOrMyList(bool VeritabaniMi)
        {
            //kaç satırlık veri var bunların değerini çekiyorum.
            int sonsatirMyList = sutunAVeri.Count();
            int sonsatirVT = columnKelimeVeri.Count();

            int hangiSatirMyList = rn.Next(0, sonsatirMyList);//veri sayısına göre rastgele bir satırdan veri çekmeyi istiyorum.
            int hangiSatirVT = rn.Next(0, sonsatirVT);

            string kelime = VeritabaniMi ? columnKelimeVeri[hangiSatirVT][0].ToString() : sutunAVeri[hangiSatirMyList][0].ToString();//Eğer veritabaniMi sorgusu true gelirse kendi sözlüğümden veri çekip değişkene atayacağım değilse sayfa1'de bulunan kendi eklediğim kelimelerden veri çekip değişkene atayacağım.
            string word = VeritabaniMi ? columnWordData[hangiSatirVT][0].ToString() : sutunBVeri[hangiSatirMyList][0].ToString();//aynı mantıkla kelimenin anlamını çekiyorum.

            return Tuple.Create(KelimeDuzelt(kelime), KelimeDuzelt(word));//verileri Tuple nesnesine çevirip gönderiyorum.
        }
        public string[] Rastgele4KelimeGetir(string dogruKelime)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {
            int sonsatirVT = columnKelimeVeri.Count();
            int eklenenIndex = 0;
            string[] kelimeler = new string[4];

            while (eklenenIndex < 4)
            {
                int hangiSatirVTT = rn.Next(0, sonsatirVT);
                string gelenVeri = columnKelimeVeri[hangiSatirVTT][0].ToString();
                if (!kelimeler.Contains(gelenVeri) && gelenVeri != dogruKelime)
                {
                    kelimeler[eklenenIndex] = gelenVeri;
                    eklenenIndex++;
                }
            }

            return kelimeler;
        }

        public void VeriEkle(string word, string kelime)
        {
            // KelimeAra metodunu asenkron olarak çağırın
            var kelimeAraResult = KelimeAra(word);

            string[] words = kelimeAraResult.Item1.ToArray();
            string[] kelimeler = kelimeAraResult.Item2.ToArray();

            for (int i = 0; i < words.Length; i++)
            {
                if (word.ToLower() == words[i].ToLower())//Eğer database'imde aradığım kelime varsa bu çalışacak yoksa aşağıdaki
                {
                    kelime = KelimeDuzelt(kelimeler[i]);
                }
                else
                {
                    kelime = Ceviri(word, false).ToString(); // EnglishToTurkish metodunu kullanarak arama yapıyorum.
                }
            }
            // Veriyi eklemek için gereken parametreleri oluştur
            body = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { KelimeDuzelt(word), KelimeDuzelt(kelime) } }
            };

            verireq = service.Spreadsheets.Values.Append(body, spreadsheetId, range);//Veri ekleme

            // Veriyi eklemek istediğiniz hücreyi belirleyin

            // Veriyi ekle
            verireq.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            verireq.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

            AppendValuesResponse response = verireq.Execute();

            // İşlem sonucunu kontrol edin
            //if (responseUpdate.UpdatedCells > 0)
            //{
            //    Console.WriteLine("Veri başarıyla eklendi.");//Değişebilir
            //}
            //else
            //{
            //Console.WriteLine("Veri ekleme başarısız.");
            //}
        }

        public Tuple<List<string>, List<string>> Sayfa1Veri()
        {
            List<string> ASutunu = new List<string>();
            List<string> BSutunu = new List<string>();

            if (sutunAVeri != null && sutunAVeri.Count > 0)
            {
                foreach (var row in sutunAVeri)
                {
                    foreach (string cell in row)
                    {
                        ASutunu.Add(cell);
                    }
                }
            }
            if (sutunBVeri != null && sutunBVeri.Count > 0)
            {
                foreach (var row in sutunBVeri)
                {
                    foreach (string cell in row)
                    {
                        BSutunu.Add(cell);
                    }
                }
            }
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
            return sonuc;
        }
    }

}
