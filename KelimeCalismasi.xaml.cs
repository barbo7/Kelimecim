namespace Kelimecim;

public partial class KelimeCalismasi : ContentPage
{
    GoogleSheets gs = GoogleSheets.Instance;
    CancellationTokenSource cancelTokenSource;
    Tuple<string, string> veri;
    public KelimeCalismasi()
	{
		InitializeComponent();
	}

    //geriye gelme özelliði de eklenebilir.
    private void SagButton_Clicked(object sender, EventArgs e)
    {
        if(!gs.kelimeSayfasiHazirMi)
        {
            DisplayAlert("Uyarý", "Sayfa henüz hazýr deðil tekrar deneyin", "Tamam");
            return;
        }

        veri = gs.RastgeleKelimeGetirVTOrMyList(true);

        wordText.Text = veri.Item2;
        kelimeText.Text = veri.Item1;
        MetindenSese(wordText.Text);
       
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


    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (wordText != null && wordText.Text != null)
            MetindenSese(wordText.Text);
    }
}