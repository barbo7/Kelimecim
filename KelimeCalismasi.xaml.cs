namespace Kelimecim;

public partial class KelimeCalismasi : ContentPage
{
    GoogleSheets gs = GoogleSheets.Instance;
    Tuple<string, string> veri;
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


    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (wordText.Text is not null)
            await TextToSpeech.SpeakAsync(wordText.Text);
    }
}