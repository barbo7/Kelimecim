
namespace Kelimecim
{
    public partial class KelimeListesi : ContentPage
    {
        GoogleSheets gs = GoogleSheets.Instance;
        CancellationTokenSource cancelTokenSource;

        public string KelimeListesiRoute { get; set; }

        public KelimeListesi()
        {
            InitializeComponent();
            OnAppearing(); // �lk y�kleme i�lemleri
        }

        protected override void OnAppearing()
        {
            SayfayiAc();
            SayfayiYenidenYukle();
        }
        void SayfayiYenidenYukle()
        {
            // Sayfa i�eri�ini temizle
            kelimeAnlamlariGrid.Children.Clear();
            kelimeAnlamlariGrid.RowDefinitions.Clear();
            kelimeAnlamlariGrid.ColumnDefinitions.Clear();

            // Yeniden olu�tur
            SayfayiAc();
        }

        private async void MetindenSese(string textBoxText)
        {
            // �ptal belirteci (CancellationToken) olu�tur
            if (cancelTokenSource != null)
            {
                // �nceki okuma i�lemini iptal et
                cancelTokenSource.Cancel();
            }

            // Yeni bir iptal belirteci olu�tur
            cancelTokenSource = new CancellationTokenSource();

            await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
        }
        void SayfayiAc()
        {
            List<string> sutunA = gs.Sayfa1Veri().Item1;
            List<string> sutunB = gs.Sayfa1Veri().Item2;

            // Sayfa i�eri�ini ScrollView'a ayarlay�n

            kelimeAnlamlariGrid.ColumnSpacing = 10; // H�creler aras�ndaki yatay bo�lu�u ayarla

            for (int i = 0; i < sutunA.Count; i++)
            {
                var kelimeLabel = new Label { Text = sutunA[i], FontSize = 12, WidthRequest=150, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start };
                var anlamLabel = new Label { Text = sutunB[i], FontSize = 10, WidthRequest = 150, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Start };
                var textToSpeechButton = new ImageButton { Source = "sound.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };
                var removeButton = new ImageButton { Source = "remove.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };

                // ImageButton'a t�klanma olay�n� ekleyin
                textToSpeechButton.Clicked += (sender, e) =>
                {
                    MetindenSese(kelimeLabel.Text);
                };
                removeButton.Clicked += (sender, e) =>
                {
                    silmeEmri(kelimeLabel.Text);
                };


                // Grid i�erisinde sat�r ve s�tunlar olu�turuyoruz
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = 10 }); // Bo�luk ekleyen sat�r

                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


                // Grid.SetRow ve Grid.SetColumn ile sat�r ve s�tun belirtiyoruz
                Grid.SetRow(kelimeLabel, i * 2); // Her iki sat�r aras�nda bir bo�luk oldu�u i�in �ift say�lar�n� kullan�yoruz
                Grid.SetColumn(kelimeLabel, 0);

                Grid.SetRow(anlamLabel, i * 2);
                Grid.SetColumn(anlamLabel, 1);

                Grid.SetRow(textToSpeechButton, i * 2);
                Grid.SetColumn(textToSpeechButton, 2);

                Grid.SetRow(removeButton, i * 2);
                Grid.SetColumn(removeButton, 3);

                // Bo�luk ekleyen sat�r�n y�ksekli�ini belirliyoruz
                kelimeAnlamlariGrid.RowDefinitions[i * 2 + 1].Height = 10;

               
                kelimeAnlamlariGrid.Children.Add(kelimeLabel);
                kelimeAnlamlariGrid.Children.Add(anlamLabel);
                kelimeAnlamlariGrid.Children.Add(textToSpeechButton);
                kelimeAnlamlariGrid.Children.Add(removeButton);
            }
        }
        private async void silmeEmri(string word)
        {
            var result = await DisplayAlert("Silme ��lemi", $"'{word}' Kelimesini silmek istedi�inize emin misiniz?", "Hay�r", "Evet");

            if (!result)
            {
                if (gs.VeriSil(word))
                {
                    // Sayfay� tekrar y�kle
                    SayfayiYenidenYukle();
                    await DisplayAlert("Ba�ar�l�", "Kelime silme i�leminiz ba�ar�l�!", "Tamam");
                }
                else
                    await DisplayAlert("Zaten silindi", "Kelime �oktan silindi. Sayfay� tekrar a��n�z.", "Tamam");
            }
        }
        // Yeni ekledi�imiz GridItem s�n�f�
    }
}
