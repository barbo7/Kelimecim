namespace Kelimecim;

public partial class CoktanSecmeli : ContentPage
{
    GoogleSheets gs = new GoogleSheets();
    private Random random = new Random();

    private RadioButton[] radioButtons; // System.Windows.Forms.RadioButton türünü kullanýyoruz

    int randomIndex;
    int yanlis = 0;
    int dogru = 0;
    int soruS = 0;

    string yaziyanlis = "Yanlýþ Sayýsý = ";
    string yazidogru = "Doðru Sayýsý = ";
    //string soru = "Soru ";

    CancellationTokenSource cancelTokenSource;
    public CoktanSecmeli()
	{
		InitializeComponent();

        // RadioButton'larýn CheckedChanged olayýna olay iþleyici ekleyin
        radioButton1.CheckedChanged += RadioButton_CheckedChanged;
        radioButton2.CheckedChanged += RadioButton_CheckedChanged;
        radioButton3.CheckedChanged += RadioButton_CheckedChanged;
        radioButton4.CheckedChanged += RadioButton_CheckedChanged;
        radioButton5.CheckedChanged += RadioButton_CheckedChanged;

        radioButtons = new RadioButton[] { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5 };
        sirala();
        dogruSayisi.Text = yazidogru + dogru;
        yanlisSayisi.Text = yaziyanlis + yanlis;
        Thread.Sleep(3200);
        MetindenSese(word.Text);
    }

    private async void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton rb = (RadioButton)sender;//Hangi button'a týklandýðýný anlayýp iþlem yapmak için bir deðiþkene atýyorum.
        if (rb.IsChecked)
        {
            radioButtons[randomIndex].TextColor = Color.FromRgb(0, 255, 0);//doðru olan cevabý yeþil iþaretliyorum.


            if (rb != radioButtons[randomIndex])
            {
                rb.TextColor = Color.FromRgb(255, 0, 0);//eðer doðru cevap deðil ise kýrmýzý renkte olsun iþaretlediðim.
                yanlis++;
                yanlisSayisi.Text = yaziyanlis + yanlis;
            }
            else
            {
                dogru++;
                dogruSayisi.Text = yazidogru + dogru;
            }
            //label6.Text = "Doðruluk oraný = %" + (dogru * 100 / (dogru + yanlis)).ToString();
            MetindenSese(word.Text + " " + radioButtons[randomIndex].Content);
            for (int i = 0; i < radioButtons.Length; i++)
                radioButtons[i].IsEnabled = false;//kullanýcý baþka bir seçeneðe týklamasýn diye buttonlarýn týklanabilirlik özelliðini kapatýyorum.

            await Task.Delay(2000); // 3 saniye bekleniyor kullanýcý doðru cevabý görsün diye

            rb.IsChecked = false;//seçili olan radiobutton kaldýrýyorum.
                                 // Bekleme süresi sonrasýnda yapýlacak iþ
            sirala();//yeni þýklarý getiriyorum.

            for (int i = 0; i < radioButtons.Length; i++)
            {
                radioButtons[i].TextColor = Color.FromRgb(128, 128, 128);
                //diðer buttonlarýn renklerini düzenliyorum ve aþaðýdaki kodda da buttonlarý aktif ediyorum.
                radioButtons[i].IsEnabled = true;
            }
            await Task.Delay(1500);
            MetindenSese(word.Text);
        }
    }

    private void sirala()
    {
        Tuple<string, string> dogruCevap = gs.RastgeleKelimeGetirVTOrMyList(true);

        soruS++;
        word.Text = dogruCevap.Item2;
        //label5.Text = soru + soruS;

        // Rastgele bir RadioButton seçin
        randomIndex = random.Next(0, radioButtons.Length); // 0 ile (RadioButton dizisinin uzunluðu - 1) arasýnda rastgele bir indeks seçin

        for (int i = 0; i < radioButtons.Length; i++)
        {
            if (i != randomIndex)
            {
                Tuple<string, string> yanlisCevap = gs.RastgeleKelimeGetirVTOrMyList(true);
                radioButtons[i].Content = yanlisCevap.Item1;//eðer seçilen rastgele button deðil ise diðer buttonlara deðer giriyorum rastgele.
            }
            else
                radioButtons[i].Content = dogruCevap.Item1;//belirlediðim cevabý atýyorum.
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

        await TextToSpeech.SpeakAsync(textBoxText, cancelTokenSource.Token);
    }
    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        MetindenSese(word.Text);
    }
}