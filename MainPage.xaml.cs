namespace Kelimecim
{
    public partial class MainPage : ContentPage
    {
        GoogleSheets gs = GoogleSheets.Instance;
        public MainPage()
        {

            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            bool translateMi = EngTr.IsChecked;
            kelimeWordShowPlace.Text = translateMi
                ? gs.KelimeAra(kelimeWordEntry.Text).Item2[0]
                : gs.KelimeAra2(kelimeWordEntry.Text, true);


            string okunacakKelime = translateMi ? kelimeWordEntry.Text : kelimeWordShowPlace.Text;
            await TextToSpeech.SpeakAsync(okunacakKelime);
        }

    }
}
