
namespace Kelimecim
{
    public partial class MainPage : ContentPage
    {
        GoogleSheets gs = GoogleSheets.Instance;
        CancellationTokenSource cancelTokenSource;

        public MainPage()
        {

            InitializeComponent();
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
        private void OnCounterClicked(object sender, EventArgs e)
        {
            Arama();
        }
       
        private void OnKelimeEntryCompleted(object sender, EventArgs e)
        {
            Arama();
        }
        private async void Arama()
        {
            bool translateMi = EngTr.IsChecked;
            kelimeWordShowPlace.Text = translateMi
                ? gs.KelimeAra(kelimeWordEntry.Text).Item2[0]
                : gs.Ceviri(kelimeWordEntry.Text, true).ToString();


            string okunacakKelime = translateMi ? kelimeWordEntry.Text : kelimeWordShowPlace.Text;
            await TextToSpeech.SpeakAsync(okunacakKelime);
            if (kelimeWordEntry.Text == kelimeWordShowPlace.Text)
            {
                kelimeWordShowPlace.Text += "(!!!)";
                await DisplayAlert("Uyarı", "Aradığınız kelime/cümle hatalı olabilir!", "Tamam");
            }
        }
    }
}
