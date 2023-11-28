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
            // Ýptal belirteci (CancellationToken) oluþtur
            if (cancelTokenSource != null)
            {
                // Önceki okuma iþlemini iptal et
                cancelTokenSource.Cancel();
            }

            // Yeni bir iptal belirteci oluþtur
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
                Orientation = ScrollOrientation.Horizontal, // Yatay kaydýrma ekleyin
                Content = kelimeAnlamlariGrid
            };

            // Sayfa içeriðini ScrollView'a ayarlayýn
            Content = scrollView;

            kelimeAnlamlariGrid.ColumnSpacing = 10; // Hücreler arasýndaki yatay boþluðu ayarla

            for (int i = 0; i < sutunA.Count; i++)
            {
                var kelimeLabel = new Label { Text = sutunA[i], FontSize = 16, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start };
                var anlamLabel = new Label { Text = sutunB[i], FontSize = 16, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Start };
                var textToSpeechButton = new ImageButton { Source = "sound.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End,  };

                // Grid içerisinde satýr ve sütunlar oluþturuyoruz
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                kelimeAnlamlariGrid.RowDefinitions.Add(new RowDefinition { Height = 10 }); // Boþluk ekleyen satýr

                kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Grid.SetRow ve Grid.SetColumn ile satýr ve sütun belirtiyoruz
                Grid.SetRow(kelimeLabel, i * 2); // Her iki satýr arasýnda bir boþluk olduðu için çift sayýlarýný kullanýyoruz
                Grid.SetColumn(kelimeLabel, 0);

                Grid.SetRow(anlamLabel, i * 2);
                Grid.SetColumn(anlamLabel, 1);

                Grid.SetRow(textToSpeechButton, i * 2);
                Grid.SetColumn(textToSpeechButton, 2);

                // Boþluk ekleyen satýrýn yüksekliðini belirliyoruz
                kelimeAnlamlariGrid.RowDefinitions[i * 2 + 1].Height = 10;

                // ImageButton'a týklanma olayýný ekleyin
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
