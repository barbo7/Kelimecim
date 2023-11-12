using System.Reflection;

namespace Kelimecim
{
    public partial class MainPage : ContentPage
    {
        GoogleSheets gs;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            try
            {
                gs = new GoogleSheets();
                // GoogleSheets sınıfını kullanmaya devam et
            }
            catch (TargetInvocationException ex)
            {
                // İstisna detaylarını görüntüle
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Console.WriteLine(innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
            }

            bool translateMi = EngTr.IsChecked;//Eğer bu true gelirse ing->tr false elirse tr->ing
            if (kelimeWordEntry.Text != "")
            {
                kelimeWordShowPlace.Text = translateMi ? gs.KelimeAra(kelimeWordEntry.Text).Item2[0] : gs.KelimeAra(kelimeWordEntry.Text, true);//BUNLAR HATALI.
                string okunacakKelime = translateMi ? kelimeWordEntry.Text : kelimeWordShowPlace.Text;
                await TextToSpeech.SpeakAsync(okunacakKelime);
            }
        }
    }
}
