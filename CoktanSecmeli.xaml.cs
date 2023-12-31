namespace Kelimecim;

public partial class CoktanSecmeli : ContentPage
{
    GoogleSheets gs = GoogleSheets.Instance;
    VeriYonlendir vy = VeriYonlendir.Instance;

    private Random random = new Random();

    private RadioButton[] radioButtons; // System.Windows.Forms.RadioButton t�r�n� kullan�yoruz

    int randomIndex;
    int yanlis = 0;
    int dogru = 0;
    int soruS = 0;

    string yaziyanlis = "Yanl�� Say�s� = ";
    string yazidogru = "Do�ru Say�s� = ";
    //string soru = "Soru ";

    bool buttonStarted = false;
    bool tersCevirildiMi = false;

    CancellationTokenSource cancelTokenSource;
    public CoktanSecmeli()
	{
		InitializeComponent();

        // RadioButton'lar�n CheckedChanged olay�na olay i�leyici ekleyin
        radioButton1.CheckedChanged += RadioButton_CheckedChanged;
        radioButton2.CheckedChanged += RadioButton_CheckedChanged;
        radioButton3.CheckedChanged += RadioButton_CheckedChanged;
        radioButton4.CheckedChanged += RadioButton_CheckedChanged;
        radioButton5.CheckedChanged += RadioButton_CheckedChanged;

        radioButtons = new RadioButton[] { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5 };
        foreach (var i in radioButtons)
            i.IsEnabled = false;
    }
    private async void SayfayiAc()
    {
        int denemeS = 0;
        while (!gs.CoktanSecmeliSayfasiHazirMi)
        {
            await Task.Delay(500);
            if(denemeS==0)
            {
                await DisplayAlert("Uyar�", "Sayfa hen�z haz�r de�il tekrar deneyin", "Tamam");
                denemeS++;
            }
        }

        sirala();
        dogruSayisi.Text = yazidogru + dogru;
        yanlisSayisi.Text = yaziyanlis + yanlis;
    }



    private async void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton rb = (RadioButton)sender;//Hangi button'a t�kland���n� anlay�p i�lem yapmak i�in bir de�i�kene at�yorum.
        if (rb.IsChecked)
        {
            radioButtons[randomIndex].TextColor = Color.FromRgb(0, 255, 0);//do�ru olan cevab� ye�il i�aretliyorum.

            //gs.gosterilenKelimelerDogru.Add(radioButtons[randomIndex].Content.ToString());
           //Do�ru cevab� kullan�c� listesine eklemek isterse diye sonras�nda listelemek i�in ekliyorum.

            if (rb != radioButtons[randomIndex])
            {
               

                rb.TextColor = Color.FromRgb(255, 0, 0);//e�er do�ru cevap de�il ise k�rm�z� renkte olsun i�aretledi�im.
                yanlis++;
                yanlisSayisi.Text = yaziyanlis + yanlis;
                string veri = tersCevirildiMi ? rb.Content.ToString() : gs.KelimeAraTR(rb.Content.ToString());
                vy.gosterilenKelimelerYanlis.Add(veri);
            }
            else
            {
                dogru++;
                dogruSayisi.Text = yazidogru + dogru;
            }


            //label6.Text = "Do�ruluk oran� = %" + (dogru * 100 / (dogru + yanlis)).ToString();
            MetindenSese(word.Text + " " + radioButtons[randomIndex].Content);
            for (int i = 0; i < radioButtons.Length; i++)
                radioButtons[i].IsEnabled = false;//kullan�c� ba�ka bir se�ene�e t�klamas�n diye buttonlar�n t�klanabilirlik �zelli�ini kapat�yorum.

            await Task.Delay(2000); // 3 saniye bekleniyor kullan�c� do�ru cevab� g�rs�n diye

            rb.IsChecked = false;//se�ili olan radiobutton kald�r�yorum.
                                 // Bekleme s�resi sonras�nda yap�lacak i�
            sirala();//yeni ��klar� getiriyorum.

            for (int i = 0; i < radioButtons.Length; i++)
            {
                radioButtons[i].TextColor = Color.FromRgb(128, 128, 128);
                //di�er buttonlar�n renklerini d�zenliyorum ve a�a��daki kodda da buttonlar� aktif ediyorum.
                radioButtons[i].IsEnabled = true;
            }
        }
    }

    private void sirala()
    {
        Tuple<string, string> dogruCevap = gs.RastgeleKelimeGetirVTOrMyList(true);
        //string[] yanlisKelimeler = gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item1, false);
        //string[] yanlisWordler = gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item2, true);

        string[] yanlislar = tersCevirildiMi ? gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item2, true) : gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item1, false);
        //E�er tersCevirildiMi switch'ine dokunulmad�ysa ingilizce kelimeyi tahmin ediyoruz i�aretlendi�inde t�rk�e kelimeyi tahmin ediyoruz.

        int indexx = 0;
        soruS++;
        word.Text = tersCevirildiMi ? dogruCevap.Item1 : dogruCevap.Item2; // ing mi tr mi
        //label5.Text = soru + soruS;

        vy.gosterilenKelimelerDogru.Add(dogruCevap.Item2);

        // Rastgele bir RadioButton se�in
        randomIndex = random.Next(0, radioButtons.Length); // 0 ile (RadioButton dizisinin uzunlu�u - 1) aras�nda rastgele bir indeks se�in

        for (int i = 0; i < radioButtons.Length; i++)
        {
            if (i != randomIndex)
            {
                radioButtons[i].Content = yanlislar[indexx]; ;//e�er se�ilen rastgele button de�il ise di�er buttonlara de�er giriyorum rastgele.
                indexx++;
            }
            else
                radioButtons[i].Content = tersCevirildiMi ? dogruCevap.Item2 : dogruCevap.Item1;//belirledi�im cevab� at�yorum.
        }
        MetindenSese(word.Text);
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
        MetindenSese(word.Text);
    }

    private void switchCevir(object sender, ToggledEventArgs e)
    {
        tersCevirildiMi = switchCevirr.On;
        switchCevirr.Text = tersCevirildiMi ? "Ters �evir" : "Reverse it";
        sirala();
        
        // ��aretlenip i�aretlenmedi�i ile ilgili i�lemleri burada yapabilirsiniz
    }

    private async void BaslatButton_Clicked(object sender,EventArgs e)
    {
        if (!buttonStarted)
        {
            foreach (var i in radioButtons)
                i.IsEnabled = true;
            SayfayiAc();
            buttonStarted = true;
            button.Text = "Bitir";
        }
        else
        {
            int sorus = dogru + yanlis;
            if (sorus == 0)
                return;

            var result = await DisplayAlert("Listeye Ekle", "Sayfaya y�nlendiriliyorsunuz", "Eklemeden ��k", "Devam et");

            if (!result)
            {
                vy.trMii = tersCevirildiMi;//false ise ing true ise tr

                await Navigation.PushAsync(new SorguSayfasi());
                // Kullan�c� "Evet" butonuna t�klad�
            }
            else
            {
                SayfayiAc();
                vy.gosterilenKelimelerDogru.Clear();
                vy.gosterilenKelimelerYanlis.Clear();
            }
        }

    }
}