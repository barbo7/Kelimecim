namespace Kelimecim;

public partial class KelimeCalismasi : ContentPage
{
    //GoogleSheets gs = GoogleSheets.Instance;
    SqliteProcess sp = SqliteProcess.Instance;
    CancellationTokenSource cancelTokenSource;
    int dataset = 4;
    Tuple<string, string> veri;
    public KelimeCalismasi()
    {
        InitializeComponent();
        List<string> veriSetleri = new List<string> { "A1-B2 Eng", "B2-C1 Eng", "Deutsch", "Mixed Eng" };
        picker.ItemsSource = veriSetleri;
        YeniKelime();
    }
    private async void SolButton_Clicked(object sender, EventArgs e)
    {
        if (wordText.Text != "")
        {
            sp.VeriEkle(wordText.Text, kelimeText.Text);
            YeniKelime();
        }
    }
    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        dataset = picker.SelectedIndex;

        if (picker.SelectedIndex == 0)//a1-b2
            dataset = 1;
        else if (picker.SelectedIndex == 1)//b2-c1
            dataset = 2;
        else if (picker.SelectedIndex == 2)//deutsch
            dataset = 3;
        else if (picker.SelectedIndex == 3)//mixed
            dataset = 4;
        YeniKelime();
    }
    //geriye gelme özelliði de eklenebilir.
    private void SagButton_Clicked(object sender, EventArgs e)
    {
        //if(!gs.kelimeSayfasiHazirMi)
        //{
        //    DisplayAlert("Uyarý", "Sayfa henüz hazýr deðil tekrar deneyin", "Tamam");
        //    return;
        //}

        YeniKelime();
    }
    private void YeniKelime()
    {
        //Kullanıcının isteğine göre cümleler çekilecek anlamlarıyla birlikte. Yeni sayfada

        veri = sp.RastgeleKelimeGetirVeriSeti(dataset);

        wordText.Text = veri.Item1;
        kelimeText.Text = veri.Item2;
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

        try
        {
            await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
        }
        catch (Exception ex)
        {

        }
    }


    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        if (wordText != null && wordText.Text != null)
            MetindenSese(wordText.Text);
    }
}