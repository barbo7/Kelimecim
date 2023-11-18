namespace Kelimecim;

public partial class CumleCalismasi : ContentPage
{
    GoogleSheets gs = new GoogleSheets();
    CancellationTokenSource cancelTokenSource;
    public CumleCalismasi()
    {
        InitializeComponent();
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

    private void SagButton_Clicked(object sender, EventArgs e)
    {

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