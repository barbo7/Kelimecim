﻿namespace Kelimecim;

public partial class CumleCalismasi : ContentPage
{
    //GoogleSheets gs = GoogleSheets.Instance;
    SqliteProcess sp = SqliteProcess.Instance;
    CancellationTokenSource cancelTokenSource;
    public CumleCalismasi()
    {
        InitializeComponent();
        CumleGetir();
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

        try
        {
            await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
        }
        catch (Exception ex)
        {

        }
    }
    private async void SolButton_Clicked(object sender, EventArgs e)
    {
        if (wordText.Text != "")
        {
            sp.VeriEkle(wordText.Text, kelimeText.Text);
            CumleGetir();
        }
    }

    private void SagButton_Clicked(object sender, EventArgs e)
    {
        // if(!gs.CumleSayfasiHazirMi)
        //{
        //    DisplayAlert("Uyarý", "Sayfa henüz hazýr deðil tekrar deneyin", "Tamam");
        //    return;
        //}

        CumleGetir();
    }

    private void CumleGetir()
    {
        Tuple<string, string, string> vericik = sp.RastgeleCumle();

        sentences.Text = UpdateEditorText(vericik.Item1);
        wordText.Text = vericik.Item2;
        kelimeText.Text = vericik.Item3;
        MetindenSese(wordText.Text);
    }
    private string UpdateEditorText(string input)
    {
        // Görülecek karakterleri belirle
        char[] delimiters = { '1', '2', '3' };

        // Metni belirtilen karakterlere göre böl ve boş string'leri dikkate alma
        string[] parts = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        // Yeni satır karakterleri ekleyerek parçaları birleştir
        string newText = string.Join(Environment.NewLine, parts);

        // Editor'ün Text özelliğini güncelle
        return newText;
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