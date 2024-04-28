
namespace Kelimecim
{
    public partial class KelimeListesi : ContentPage
    {
        //GoogleSheets gs = GoogleSheets.Instance;
        SqliteProcess sp = SqliteProcess.Instance;
        CancellationTokenSource cancelTokenSource;

        int currentPage = 1;
        int pageSize = 20; // Bir sayfada gösterilecek maksimum kelime sayısı
        int totalPage = 1; // Toplam sayfa sayısı

        List<string> sutunA = new List<string>();
        List<string> sutunB = new List<string>();

        public string KelimeListesiRoute { get; set; }

        public KelimeListesi()
        {
            InitializeComponent();
            OnAppearing(); // Ýlk yükleme iþlemleriKm
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();  // Base class'ın OnAppearing metodunu da çağırın
            CalculateTotalPage();  // Toplam sayfa sayısını hesapla
            LoadPage(currentPage);  // İlk sayfayı yükle
        }

        private async void MetindenSese(string textBoxText)
        {
            // Ýptal belirteci (CancellationToken) oluþtur
            if (cancelTokenSource != null)
            {
                // Önceki okuma iþlemini iptal et
                cancelTokenSource.Cancel();
            }

            // Yeni bir iptal belirteci oluþtur
            cancelTokenSource = new CancellationTokenSource();

            try
            {
                await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
            }
            catch (Exception ex)
            {

            }
        }
        void LoadPage(int page)
        {
            sutunA = sp.Sayfa1Veri().Item1;
            sutunB = sp.Sayfa1Veri().Item2;

            int skip = (page - 1) * pageSize; // Atlanacak kelime sayısı
            int take = pageSize; // Alınacak kelime sayısı

            var pageItemsA = sutunA.Skip(skip).Take(take).ToList();
            var pageItemsB = sutunB.Skip(skip).Take(take).ToList();

            RenderWords(pageItemsA, pageItemsB);
            UpdateNavigationButtons();  // Navigasyon butonlarını güncelle
        }
        void RenderWords(List<string> words, List<string> meanings)
        {
            kelimeAnlamlariGrid.Children.Clear();
            kelimeAnlamlariGrid.RowDefinitions.Clear();
            kelimeAnlamlariGrid.ColumnDefinitions.Clear();

            // Sütunları yeniden tanımla
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


            for (int i = 0; i < words.Count; i++)
            {
                var kelimeLabel = new Label { Text = words[i], FontSize = 12, WidthRequest = 150, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start };
                var anlamLabel = new Label { Text = meanings[i], FontSize = 10, WidthRequest = 150, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Start };

                var textToSpeechButton = new ImageButton { Source = "sound.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };
                var removeButton = new ImageButton { Source = "remove.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };

                textToSpeechButton.Clicked += (sender, e) => MetindenSese(kelimeLabel.Text);
                removeButton.Clicked += (sender, e) => silmeEmri(kelimeLabel.Text);

                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Grid.SetRow(kelimeLabel, i);
                Grid.SetColumn(kelimeLabel, 0);

                Grid.SetRow(anlamLabel, i);
                Grid.SetColumn(anlamLabel, 1);

                Grid.SetRow(textToSpeechButton, i);
                Grid.SetColumn(textToSpeechButton, 2);

                Grid.SetRow(removeButton, i);
                Grid.SetColumn(removeButton, 3);

                kelimeAnlamlariGrid.Children.Add(kelimeLabel);
                kelimeAnlamlariGrid.Children.Add(anlamLabel);
                kelimeAnlamlariGrid.Children.Add(textToSpeechButton);
                kelimeAnlamlariGrid.Children.Add(removeButton);
            }
        }

        private void KelimeAra_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = e.NewTextValue;  // Kullanıcının girdiği yeni metin
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                SearchInDatabase(searchQuery);
            }
            else
            {
                LoadPage(currentPage);  // Eğer arama metni boşsa, mevcut sayfayı yeniden yükle
            }
        }

        private void SearchInDatabase(string query)
        {
            // Örneğin, SqliteProcess sınıfında bir arama fonksiyonu çağrılabilir:
            var results = sp.SearchWords(query);  // Veritabanında arama yap
            var words = results.Select(r => r.Word).ToList();
            var meanings = results.Select(r => r.Meaning).ToList();

            RenderWords(words, meanings);  // Arama sonuçlarını göster
        }

        private async void silmeEmri(string word)
        {
            var result = await DisplayAlert("Silme İşlemi", $"'{word}' Kelimesini silmek istediðinize emin misiniz?", "Evet", "Hayır");

            if (result)
            {
                if (sp.VeriSil(word))
                {
                    LoadPage(currentPage);
                }
                else
                    await DisplayAlert("Zaten silindi", "Kelime çoktan silindi. Sayfayı tekrar açınız", "Tamam");
            }
        }
        void CalculateTotalPage()
        {
            int totalItems = sp.Sayfa1Veri().Item1.Count; // Toplam kelime sayısı
            totalPage = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        private void sonrakiSayfa_Clicked(object sender, EventArgs e)
        {
            if (currentPage < totalPage)
            {
                currentPage++;
                LoadPage(currentPage);
            }
        }
        private void UpdateNavigationButtons()
        {
            ilkSayfa.IsEnabled = currentPage > 1;
            oncekiSayfa.IsEnabled = currentPage > 1;
            sonrakiSayfa.IsEnabled = currentPage < totalPage;
            sonSayfa.IsEnabled = currentPage < totalPage;
        }

        private void oncekiSayfa_Clicked(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPage(currentPage);
            }
        }

        private void ilkSayfa_Clicked(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadPage(currentPage);
        }
        private void sonSayfa_Clicked(object sender, EventArgs e)
        {
            currentPage = totalPage;
            LoadPage(currentPage);
        }

    }
}
