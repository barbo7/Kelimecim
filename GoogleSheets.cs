using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Collections;
using System.Linq;

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

        //ValueRange responseSutunA;
        //ValueRange responseSutunB;
        //ValueRange responseSearchWord;
        //ValueRange responseAramaKelime;
        //ValueRange responseCumle;
        //ValueRange responseCumleWord;
        //ValueRange responseCumleKelime;

        IList<IList<object>> sutunAVeri;
        IList<IList<object>> sutunBVeri;
        IList<IList<object>> columnWordData;
        IList<IList<object>> columnKelimeVeri;
        IList<IList<object>> cumleliSayfaCumle;
        IList<IList<object>> cumleliSayfaWord;
        IList<IList<object>> cumleliSayfaKelime;


        string spreadsheetId = "1kafz2KAuvxqSdGbfNOou1S5keIf5wQDIDRLsdm9t6l8"; // excel tablosunun id'si

        string kendiSutunum = "Sayfa1!A:B"; //hangi satırı yazmak istediğim.
        string veriKumesi = "Veriler!A:B";
        string cumlelerKumesi = "Cumleler!A:C";

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

            // Google E-Tablolar API'sini çalıştırma
            service = new SheetsService(initializer);

            // Create requests for each data range
            var requests = new List<SpreadsheetsResource.ValuesResource.GetRequest>
            {
                service.Spreadsheets.Values.Get(spreadsheetId, kendiSutunum),
                service.Spreadsheets.Values.Get(spreadsheetId, veriKumesi),
                service.Spreadsheets.Values.Get(spreadsheetId, cumlelerKumesi),
                //Bir önceki sürümde bütün sayfalar için hücre hücre veri çekme isteği oluşturuyordum. Bunun yerine tek bir sayfadaki tüm verileri çekmek için yeni bir geliştirme yaptım. Performans arttırma amaçlı
            };

            // Execute requests in parallel and wait for all to complete
            var responses = await Task.WhenAll(requests.Select(request => request.ExecuteAsync()));

            var kendiSutunumDegerleri = responses[0].Values;
            var veriKumesiDegerleri = responses[1].Values;
            var cumlelerKumesiDegerleri = responses[2].Values;

            sutunAVeri = kendiSutunumDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(0) }).ToList<IList<object>>(); //Linq kullanarak ilk hücredeki verileri gerektiği şekilde çekiyorum kendi listem için.
            sutunBVeri = kendiSutunumDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(1) }).ToList<IList<object>>();//Linq kullanarak ikinci hücredeki verileri gerektiği şekilde çekiyorum kendi listem için.

            columnWordData = veriKumesiDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(0) }).ToList<IList<object>>();//Veritabanında bulunan Word'ler
            columnKelimeVeri = veriKumesiDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(1) }).ToList<IList<object>>();//Veri tabanında bulunan Kelimeler

            cumleliSayfaCumle = cumlelerKumesiDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(0) }).ToList<IList<object>>();
            cumleliSayfaWord = cumlelerKumesiDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(1) }).ToList<IList<object>>();
            cumleliSayfaKelime = cumlelerKumesiDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(2) }).ToList<IList<object>>();

            ////Api'ye istek atıyorum
            //SpreadsheetsResource.ValuesResource.GetRequest request1 =
            //    service.Spreadsheets.Values.Get(spreadsheetId, sutun1);
            //SpreadsheetsResource.ValuesResource.GetRequest request2 =
            //    service.Spreadsheets.Values.Get(spreadsheetId, sutun2);

            //SpreadsheetsResource.ValuesResource.GetRequest reqSearch =
            //    service.Spreadsheets.Values.Get(spreadsheetId, veriKumesiIng);
            //SpreadsheetsResource.ValuesResource.GetRequest reqArama =
            //    service.Spreadsheets.Values.Get(spreadsheetId, veriKumesiTr);

            //SpreadsheetsResource.ValuesResource.GetRequest reqCumleC =
            //    service.Spreadsheets.Values.Get(spreadsheetId, cumlelerCumle);
            //SpreadsheetsResource.ValuesResource.GetRequest reqCumleW =
            //    service.Spreadsheets.Values.Get(spreadsheetId, cumlelerWord);
            //SpreadsheetsResource.ValuesResource.GetRequest reqCumleK =
            //    service.Spreadsheets.Values.Get(spreadsheetId, cumlelerKelime);

            //responseSutunA = await request1.ExecuteAsync();
            //responseSutunB = await request2.ExecuteAsync();

            //responseSearchWord = await reqSearch.ExecuteAsync();
            //responseAramaKelime = await reqArama.ExecuteAsync();

            //responseCumle = await reqCumleC.ExecuteAsync();
            //responseCumleWord = await reqCumleW.ExecuteAsync();
            //responseCumleKelime = await reqCumleK.ExecuteAsync();

            ////değerleri listelere çekiyorum.
            //sutunAVeri = responseSutunA.Values;
            //sutunBVeri = responseSutunB.Values;
            //columnWordData = responseSearchWord.Values;
            //columnKelimeVeri = responseAramaKelime.Values;
            //cumleliSayfaCumle = responseCumle.Values;
            //cumleliSayfaWord = responseCumleWord.Values;
            //cumleliSayfaKelime = responseCumleKelime.Values;

            if (columnWordData.Count > 10)
                kelimeSayfasiHazirMi = true;

            if (columnWordData.Count > 10 && sutunAVeri.Count > 1)
                CoktanSecmeliSayfasiHazirMi = true;

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

        public string KelimeAraENG(string AranacakWord)
        {
            string ingKelime = AranacakWord;
            string trKelime = null;


            var indexBul = columnWordData
            .Select((row, rowIndex) => new 
            { 
                RowIndex = rowIndex,
                Kelimeler = row.Select(kelime => kelime.ToString().ToLower())
            })
            .Where(row=>row.Kelimeler.Contains(AranacakWord.ToLower(),StringComparer.OrdinalIgnoreCase))
            .FirstOrDefault();

            if (indexBul != null)
                trKelime = columnKelimeVeri[indexBul.RowIndex][0].ToString();
            else
                trKelime = Ceviri(AranacakWord, false).ToString(); //Eğer database'imde aradığım kelime yok ise api ile çekiyorum veriyi.

            return KelimeDuzelt(trKelime);
        }


        /// <summary>
        /// Tr eng çeviri yapmak için oluşturduğum kod
        /// </summary>
        /// <param name="kelime"></param>
        /// <returns></returns>
        public string KelimeAraTR(string kelime) 
        {
            string trKelime = kelime;
            string ingKelime = null;

            var kelimeSorgusu = columnKelimeVeri
                .Select((row, rowIndex) => new
                {
                    RowIndex = rowIndex,
                    Words = row.Select(word => word.ToString().ToLower())
                })
                .Where(row => row.Words.Contains(kelime.ToLower(), StringComparer.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (kelimeSorgusu != null)
                ingKelime = columnWordData.ElementAtOrDefault(kelimeSorgusu.RowIndex)[0].ToString();
            else
                ingKelime = Ceviri(kelime, true).ToString();

            return KelimeDuzelt(ingKelime);
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
            if(!VeritabaniMi)
                Sayfa1VeriGuncelle();

            //kaç satırlık veri var bunların değerini çekiyorum.
            int sonsatirMyList = sutunAVeri.Count();
            int sonsatirVT = columnKelimeVeri.Count();

            int hangiSatirMyList = rn.Next(0, sonsatirMyList);//veri sayısına göre rastgele bir satırdan veri çekmeyi istiyorum.
            int hangiSatirVT = rn.Next(0, sonsatirVT);

            string kelime = VeritabaniMi ? columnKelimeVeri[hangiSatirVT][0].ToString() : sutunAVeri[hangiSatirMyList][0].ToString();//Eğer veritabaniMi sorgusu true gelirse kendi sözlüğümden veri çekip değişkene atayacağım değilse sayfa1'de bulunan kendi eklediğim kelimelerden veri çekip değişkene atayacağım.
            string word = VeritabaniMi ? columnWordData[hangiSatirVT][0].ToString() : sutunBVeri[hangiSatirMyList][0].ToString();//aynı mantıkla kelimenin anlamını çekiyorum.

            return Tuple.Create(KelimeDuzelt(kelime), KelimeDuzelt(word));//verileri Tuple nesnesine çevirip gönderiyorum.
        }

        public string[] Rastgele4KelimeYaDaWordGetir(string dogruKelime, bool tersCevir)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {
            //geliştirme yapmak istersem kullanıcıdan bir tane daha değer isterim boolean olacak şekilde ve true olursa columnKelimeVeri'yi false olursa columnWordData'yı
            IList<IList<object>> veriSeti = tersCevir ? columnWordData : columnKelimeVeri;

            int sonsatirVT = veriSeti.Count();
            int eklenenIndex = 0;
            string[] kelimeler = new string[4];

            while (eklenenIndex < 4)
            {
                int hangiSatirVTT = rn.Next(0, sonsatirVT);
                string gelenVeri = KelimeDuzelt(veriSeti[hangiSatirVTT][0].ToString());
                if (!kelimeler.Contains(gelenVeri) && gelenVeri != KelimeDuzelt(dogruKelime))
                {
                    kelimeler[eklenenIndex] = KelimeDuzelt(gelenVeri);
                    eklenenIndex++;
                }
            }

            return kelimeler;
        }
        public string[] Rastgele4KelimeYaDaWordGetir(string dogruKelime)//Rastgele 4 kelime getirmemi sağlıyor eğer dogru kelime aralarında yoksa.
        {
            Sayfa1VeriGuncelle();
            //geliştirme yapmak istersem kullanıcıdan bir tane daha değer isterim boolean olacak şekilde ve true olursa columnKelimeVeri'yi false olursa columnWordData'yı
            IList<IList<object>> veriSeti = sutunBVeri;

            int sonsatirVT = veriSeti.Count();
            int eklenenIndex = 0;
            string[] kelimeler = new string[4];

            while (eklenenIndex < 4)
            {
                int hangiSatirVTT = rn.Next(0, sonsatirVT);
                string gelenVeri = KelimeDuzelt(veriSeti[hangiSatirVTT][0].ToString());
                if (!kelimeler.Contains(gelenVeri) && gelenVeri != KelimeDuzelt(dogruKelime))
                {
                    kelimeler[eklenenIndex] = gelenVeri;
                    eklenenIndex++;
                }
            }
            return kelimeler;
        }

        private bool VeriSorgu(string word)
        {
            Sayfa1VeriGuncelle();

            bool varMi = sutunAVeri
                .Any(row => row.Any(kelime => kelime.ToString().Equals(word, StringComparison.OrdinalIgnoreCase)));

            return varMi;
        }


        /// <summary>
        /// Çeviri yaptığım zaman kullanılasını istediğim method(verilerimi excel'e yerleştirmek için)
        /// </summary>
        /// <param name="word"></param>
        /// <param name="kelime"></param>
        public async Task VeriEkle(string word, string kelime)
        {
            if (VeriSorgu(word))
                return;

            body = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { KelimeDuzelt(word), kelime } }
            };

            verireq = service.Spreadsheets.Values.Append(body, spreadsheetId, kendiSutunum);

            // Veriyi ekle
            verireq.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            verireq.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

            AppendValuesResponse response = await verireq.ExecuteAsync();
        }

        public bool VeriSil(string word)
        {
            int index = -1;
            bool silindiMi = false;
            for (int i = 0; i < sutunAVeri.Count; i++)
                if (sutunAVeri[i].FirstOrDefault()?.ToString() == word)
                {
                    index = i;
                    break;
                }

            if (index != -1)
            {
                index++;

                // Silme isteği oluştur
                var silmeIstegi = new Request
                {
                    DeleteDimension = new DeleteDimensionRequest
                    {
                        Range = new DimensionRange
                        {
                            Dimension = "ROWS", // Satır boyunca sil
                            StartIndex = index - 1,
                            EndIndex = index
                        }
                    }
                };

                // Silme işlemini gerçekleştir
                service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request> { silmeIstegi }
                }, spreadsheetId).Execute();
                silindiMi = true;
                Sayfa1VeriGuncelle();
            }
            return silindiMi;
        }
        public void VeriEkle(string word)
        {
            if (VeriSorgu(word))
                return;
            string kelime = KelimeDuzelt(KelimeAraENG(word));
            word = KelimeDuzelt(word);
              
            //kelime = Ceviri(word, false).ToString(); // EnglishToTurkish metodunu kullanarak arama yapıyorum.
            // Veriyi eklemek için gereken parametreleri oluştur
            body = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { KelimeDuzelt(word), kelime } }
            };

            verireq = service.Spreadsheets.Values.Append(body, spreadsheetId, kendiSutunum);//Veri ekleme

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
        private async void Sayfa1VeriGuncelle()
        {
      
                var requests = new List<SpreadsheetsResource.ValuesResource.GetRequest>
            {
                service.Spreadsheets.Values.Get(spreadsheetId, kendiSutunum) };

            // Execute requests in parallel and wait for all to complete
            var responses = await Task.WhenAll(requests.Select(request => request.ExecuteAsync()));

            var kendiSutunumDegerleri = responses[0].Values;
            sutunAVeri = kendiSutunumDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(0) }).ToList<IList<object>>(); //Linq kullanarak ilk hücredeki verileri gerektiği şekilde çekiyorum kendi listem için.
            sutunBVeri = kendiSutunumDegerleri.Select(row => new List<object> { row.ElementAtOrDefault(1) }).ToList<IList<object>>();//Linq kullanarak ikinci hücredeki verileri gerektiği şekilde 
        }

        public Tuple<List<string>, List<string>> Sayfa1Veri()
        {
            Sayfa1VeriGuncelle();
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
            return sonuc.Trim();
        }
    }

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
    public List<string> gosterilenKelimelerDogru  { get; set; }
    public bool trMii { get; set; }

    // Yapıcı metod
    public VeriYonlendir()
    {
        // Listeleri başlat
        gosterilenKelimelerYanlis = new List<string>();
        gosterilenKelimelerDogru = new List<string>();
    }
}