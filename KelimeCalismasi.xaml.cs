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
    /// Bu method sayesinde metin okuma fonksiyonu �al��t�r�ld��� zaman e�er zaten okunan bir metin var ise duraksat�p yeni metni okutabiliyorum.
    /// </summary>
    /// <param name="textBoxText"></param>
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
        if (wordText.Text is not null)
            MetindenSese(wordText.Text);
    }
}