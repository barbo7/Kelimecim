namespace Kelimecim;

public partial class KelimeTekrar : ContentPage
{
    //GoogleSheets gs = GoogleSheets.Instance;
    SqliteProcess sp = SqliteProcess.Instance;
    CancellationTokenSource cancelTokenSource;
    Tuple<string, string> veri;
    public KelimeTekrar()
	{
		InitializeComponent();
        YeniKelime();
    }
    private void SagButton_Clicked(object sender, EventArgs e)
    {
        //if (!gs.kelimeSayfasiHazirMi)
        //{
        //    DisplayAlert("Uyar�", "Sayfa hen�z haz�r de�il tekrar deneyin", "Tamam");
        //    return;
        //}

        YeniKelime();
    }
    private void YeniKelime()
    {
        veri = sp.RastgeleKelimeGetirVTOrMyList(false);

        wordText.Text = veri.Item1;
        kelimeText.Text = veri.Item2;
        MetindenSese(wordText.Text);
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

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (wordText != null && wordText.Text != null)
            MetindenSese(wordText.Text);
    }
}