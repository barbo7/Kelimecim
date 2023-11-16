using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using System.Reflection;


namespace Kelimecim
{
    public partial class MainPage : ContentPage
    {
        GoogleSheets gs = new GoogleSheets();
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
