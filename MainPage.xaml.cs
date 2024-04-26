using Microsoft.Maui.Platform;
using System.Globalization;


namespace Kelimecim
{
    public partial class MainPage : ContentPage
    {
        //GoogleSheets gs = GoogleSheets.Instance;
        SqliteProcess sp;
        CancellationTokenSource cancelTokenSource;

        public MainPage()
        {

            InitializeComponent();
            TrEng.CheckedChanged += RadioButton_CheckedChanged;
            EngTr.CheckedChanged += RadioButton_CheckedChanged;
           
        }
        private async void dbCagir()
        {
            try
            {
                sp = SqliteProcess.Instance;
            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya göster
                await DisplayAlert("Hata", $"Veri tabanında bir hata oluştu. Uygulama ön belleğini temizlemenizi öneririz.", "Tamam");
            }
        }
        private async void MetindenSese(string textBoxText)
        {
            // İptal belirteci (CancellationToken) oluştur
            if (cancelTokenSource != null)
            {
                // Önceki okuma işlemini iptal et
                cancelTokenSource.Cancel();
            }

            // Yeni bir iptal belirteci oluştur
            cancelTokenSource = new CancellationTokenSource();
             // Set the locale for the text-to-speech output

            await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
        }

        private void kelimeWord_Clicked(object sender, EventArgs e)
        {
            if (kelimeWordEntry != null && kelimeWordEntry.Text != null)
                MetindenSese(kelimeWordEntry.Text);

        }
        private async void kelimeWordCopy_Clicked(object sender, EventArgs e)
        {
            if (kelimeWordEntry != null && kelimeWordEntry.Text != null)
            {
                await Clipboard.SetTextAsync(kelimeWordEntry.Text);

                //var notification = new NotificationRequest
                //{
                //    Title = title,
                //    Description = message,
                //    ReturningData = "optional data"
                //};

                //await NotificationCenter.Current.Show(notification);
            }
            else
            {
                // kelimeWordEntry veya kelimeWordEntry.Text null ise buraya düşer
            }
        }

        private void kelimeWordShowPlace_Clicked(object sender, EventArgs e)
        {
            if (kelimeWordShowPlace != null && kelimeWordShowPlace.Text != null)
                MetindenSese(kelimeWordShowPlace.Text);
        }
        private async void kelimeWordShowPlaceCopy_Clicked(object sender, EventArgs e)
        {
            if (kelimeWordShowPlace != null && kelimeWordShowPlace.Text != null)
            {
                await Clipboard.SetTextAsync(kelimeWordShowPlace.Text);
            }
        }
        private  void OnCounterClicked(object sender, EventArgs e)
        {
            Arama();
        }
       
        private void OnKelimeEntryCompleted(object sender, EventArgs e)
        {
            Arama();
        }
        private async void Arama()
        {
            if (kelimeWordEntry is null || string.IsNullOrWhiteSpace(kelimeWordEntry.Text))
            {
                await DisplayAlert("Uyarı", "Lütfen metin girdikten sonra arama yapınız!", "Tamam");
                return;
            }

            bool translateMi = EngTr.IsChecked;
            string query = kelimeWordEntry.Text;
            kelimeWordShowPlace.Text = translateMi
                ? sp.KelimeAraENG(query)//Türkçeye çeviri
                : sp.KelimeAraTR(query);//İngilizceye çeviri


           

            string okunacakKelime = translateMi ? kelimeWordEntry.Text : kelimeWordShowPlace.Text;
            await TextToSpeech.SpeakAsync(okunacakKelime);
            if (kelimeWordEntry.Text.Trim() == kelimeWordShowPlace.Text)
            {
                kelimeWordShowPlace.Text += "(!!!)";
                await DisplayAlert("Uyarı", "Aradığınız kelime/cümle hatalı olabilir!", "Tamam");
            }
            else
            {
                string sutunA = translateMi ? query : kelimeWordShowPlace.Text;//word
                string sutunB = translateMi ? kelimeWordShowPlace.Text : query;//kelime

                sp.VeriEkle(sutunA, sutunB);
            }
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            kelimeWordLabel.Text = EngTr.IsChecked ? "English" : "Türkçe";
            kelimeWordEntry.Placeholder = EngTr.IsChecked ? "Enter text" : "Metin girin";

            kelimeWordShowPlaceLabel.Text = EngTr.IsChecked ? "Türkçe" : "English";
            kelimeWordShowPlace.Placeholder = EngTr.IsChecked ? "Çeviri" : "Translation";
        }
        private void Entry_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(kelimeWordEntry.Text))
            {
                kelimeWordShowPlace.Text = string.Empty;
            }
        }
    }
}
