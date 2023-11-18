namespace Kelimecim;

public partial class KelimeCalismasi : ContentPage
{
    GoogleSheets gs = new GoogleSheets();
    Tuple<string, string> veri;
    CancellationTokenSource cancelTokenSource;
    public KelimeCalismasi()
	{
		InitializeComponent();
	}
    private void SagButton_Clicked(object sender, EventArgs e)
    {
        veri = gs.RastgeleKelimeGetirVTOrMyList(true);

        wordText.Text = veri.Item2;
        kelimeText.Text = veri.Item1;
    }
    /// <summary>
    /// Bu method sayesinde metin okuma fonksiyonu çalýþtýrýldýðý zaman eðer zaten okunan bir metin var ise duraksatýp yeni metni okutabiliyorum.
    /// </summary>
    /// <param name="textBoxText"></param>
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
        if (wordText.Text is not null)
            MetindenSese(wordText.Text);
    }
}