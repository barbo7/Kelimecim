namespace Kelimecim;

public partial class CumleCalismasi : ContentPage
{
    GoogleSheets gs = GoogleSheets.Instance;
    CancellationTokenSource cancelTokenSource;
    public CumleCalismasi()
    {
        InitializeComponent();
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

    private void SagButton_Clicked(object sender, EventArgs e)
    {
         if(!gs.CumleSayfasiHazirMi)
        {
            DisplayAlert("Uyar�", "Sayfa hen�z haz�r de�il tekrar deneyin", "Tamam");
            return;
        }

        Tuple<string, string, string> vericik = gs.RastgeleCumle();
        sentences.Text = vericik.Item1;
        wordText.Text = vericik.Item2;
        kelimeText.Text = vericik.Item3;
        MetindenSese(wordText.Text);
    }

    private void SentencesSoundButton_Clicked(object sender, EventArgs e)
    {
        if (sentences.Text is not null)
        {
            MetindenSese(sentences.Text);
        }
    }

    private void WordTextSoundButton_Clicked(object sender, EventArgs e)
    {
        if (wordText.Text is not null)
        {
            MetindenSese(wordText.Text);
        }
    }
}