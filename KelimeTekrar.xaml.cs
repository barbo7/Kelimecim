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
        //    DisplayAlert("Uyarý", "Sayfa henüz hazýr deðil tekrar deneyin", "Tamam");
        //    return;
        //}

        YeniKelime();
    }
    private void YeniKelime() 
    {
        if (sp.UserTablosundaKacVeriVar() < 1)
        {
            DisplayAlert("Uyarı", "Lütfen çalışmak için kelime tablonuza kelime ekleyiniz", "Tamam");
            MainThread.BeginInvokeOnMainThread(() => {
                button.IsEnabled = false;
                imageButton.IsEnabled = false;
            });

            // 10 saniye sonra butonları tekrar etkinleştirecek bir timer başlat
            Device.StartTimer(TimeSpan.FromSeconds(3), () => {
                MainThread.BeginInvokeOnMainThread(() => {
                    button.IsEnabled = true;
                    imageButton.IsEnabled = true;
                });

                // Timer'ın tekrar çalışmaması için false döndür
                return false;
            });

        }
        else if(sp.UserTablosundaKacVeriVar() > 0)
        {
            //Kullanıcının isteğine göre cümleler çekilecek anlamlarıyla birlikte.
            veri = sp.RastgeleKelimeGetirVTOrMyList(false);
            wordText.Text = veri.Item1;
            kelimeText.Text = veri.Item2;
            MetindenSese(wordText.Text);
        }
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