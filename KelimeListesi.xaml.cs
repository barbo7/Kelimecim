
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
            OnAppearing(); // Ýlk yükleme iþlemleriKm
        }

        protected override void OnAppearing()
        {
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

            kelimeAnlamlariGrid.Children.Clear();
            kelimeAnlamlariGrid.RowDefinitions.Clear();
            kelimeAnlamlariGrid.ColumnDefinitions.Clear();

            // Sütunları yeniden tanımla
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kelimeAnlamlariGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < 20; i++)
            {
                var kelimeLabel = new Label { Text = sutunA[i], FontSize = 12, WidthRequest = 150, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start };
                var anlamLabel = new Label { Text = sutunB[i], FontSize = 10, WidthRequest = 150, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Start };

                var textToSpeechButton = new ImageButton { Source = "sound.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };
                var removeButton = new ImageButton { Source = "remove.png", WidthRequest = 20, HeightRequest = 20, HorizontalOptions = LayoutOptions.End };

                // Button'ların Clicked olayını yeniden bağla
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


        private async void silmeEmri(string word)
        {
            var result = await DisplayAlert("Silme Ýþlemi", $"'{word}' Kelimesini silmek istediðinize emin misiniz?", "Hayýr", "Evet");

            if (!result)
            {
                if (gs.VeriSil(word))
                {
                    // Sayfayý tekrar yükle
                    SayfayiAc();
                    await DisplayAlert("Baþarýlý", "Kelime silme iþleminiz baþarýlý!", "Tamam");
                }
                else
                    await DisplayAlert("Zaten silindi", "Kelime çoktan silindi. Sayfayý tekrar açýnýz.", "Tamam");
            }
        }

     
        private void ilkSayfa_Clicked(object sender, EventArgs e)
        {

        }
        private void oncekiSayfa_Clicked(object sender, EventArgs e)
        {

        }
        private void sonrakiSayfa_Clicked(object sender, EventArgs e)
        {

        }
        private void sonSayfa_Clicked(object sender, EventArgs e)
        {

        }
        // Yeni eklediðimiz GridItem sýnýfý
    }
}
