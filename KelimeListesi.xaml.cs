namespace Kelimecim
{
    public partial class KelimeListesi : ContentPage
    {
        GoogleSheets gs = GoogleSheets.Instance;
        CancellationTokenSource cancelTokenSource;

        public KelimeListesi()
        {
            InitializeComponent();
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

            // ScrollView ekleyin
            var scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal, // Yatay kayd�rma ekleyin
                Content = kelimeAnlamlariGrid
            };

            // Sayfa i�eri�ini ScrollView'a ayarlay�n
            Content = scrollView;

            kelimeAnlamlariGrid.ColumnSpacing = 10; // H�creler aras�ndaki yatay bo�lu�u ayarla

            for (int i = 0; i < sutunA.Count; i++)
            {
                var kelimeLabel = new Label { Text = sutunA[i], FontSize = 16, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start };
                var anlamLabel = new Label { Text = sutunB[i], FontSize = 16, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Start };
                var textToSpeechButton = new ImageButton { Source = "sound.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End,  };

                // Grid i�erisinde sat�r ve s�tunlar olu�turuyoruz
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = 10 }); // Bo�luk ekleyen sat�r

                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Grid.SetRow ve Grid.SetColumn ile sat�r ve s�tun belirtiyoruz
                Grid.SetRow(kelimeLabel, i * 2); // Her iki sat�r aras�nda bir bo�luk oldu�u i�in �ift say�lar�n� kullan�yoruz
                Grid.SetColumn(kelimeLabel, 0);

                Grid.SetRow(anlamLabel, i * 2);
                Grid.SetColumn(anlamLabel, 1);

                Grid.SetRow(textToSpeechButton, i * 2);
                Grid.SetColumn(textToSpeechButton, 2);

                // Bo�luk ekleyen sat�r�n y�ksekli�ini belirliyoruz
                kelimeAnlamlariGrid.RowDefinitions[i * 2 + 1].Height = 10;

                // ImageButton'a t�klanma olay�n� ekleyin
                textToSpeechButton.Clicked += (sender, e) =>
                {
                    MetindenSese(kelimeLabel.Text);
                };

                kelimeAnlamlariGrid.Children.Add(kelimeLabel);
                kelimeAnlamlariGrid.Children.Add(anlamLabel);
                kelimeAnlamlariGrid.Children.Add(textToSpeechButton);
            }
        }
    }
}
