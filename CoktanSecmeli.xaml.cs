namespace Kelimecim;

public partial class CoktanSecmeli : ContentPage
{
    //GoogleSheets gs = GoogleSheets.Instance;
    SqliteProcess sp = SqliteProcess.Instance;
    VeriYonlendir vy = VeriYonlendir.Instance;

    private Random random = new Random();

    private RadioButton[] radioButtons; // System.Windows.Forms.RadioButton türünü kullanýyoruz

    int randomIndex;
    int yanlis = 0;
    int dogru = 0;
    int soruS = 0;

    string yaziyanlis = "False Count = ";
    string yazidogru = "True Count = ";
    //string soru = "Soru ";

    bool buttonStarted = false;
    bool tersCevirildiMi = false;
    bool tersCevirildiktenSonraTiklandiMi = false;


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
        foreach (var i in radioButtons)
            i.IsEnabled = false;
    }
    private async void SayfayiAc()
    {
        int denemeS = 0;
        //while (!gs.CoktanSecmeliSayfasiHazirMi)
        //{
        //    await Task.Delay(500);
        //    if(denemeS==0)
        //    {
        //        await DisplayAlert("Uyarý", "Sayfa henüz hazýr deðil tekrar deneyin", "Tamam");
        //        denemeS++;
        //    }
        //}

        sirala();
        dogruSayisi.Text = yazidogru + dogru;
        yanlisSayisi.Text = yaziyanlis + yanlis;
    }


    private async void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton rb = (RadioButton)sender;//Hangi button'a týklandýðýný anlayýp iþlem yapmak için bir deðiþkene atýyorum.
        if (rb.IsChecked)
        {
            radioButtons[randomIndex].TextColor = Color.FromRgb(0, 255, 0);//doðru olan cevabý yeþil iþaretliyorum.

            //gs.gosterilenKelimelerDogru.Add(radioButtons[randomIndex].Content.ToString());
           //Doðru cevabý kullanýcý listesine eklemek isterse diye sonrasýnda listelemek için ekliyorum.

            if (rb != radioButtons[randomIndex])
            {
               

                rb.TextColor = Color.FromRgb(255, 0, 0);//eðer doðru cevap deðil ise kýrmýzý renkte olsun iþaretlediðim.
                yanlis++;
                yanlisSayisi.Text = yaziyanlis + yanlis;
                string veri = tersCevirildiMi ? rb.Content.ToString() : sp.KelimeAraTR(rb.Content.ToString());
                vy.gosterilenKelimelerYanlis.Add(veri);
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
            tersCevirildiktenSonraTiklandiMi = true;

        }
    }

    private void sirala()
    {
        Tuple<string, string> dogruCevap = sp.RastgeleKelimeGetirVTOrMyList(true);
        //string[] yanlisKelimeler = gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item1, false);
        //string[] yanlisWordler = gs.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item2, true);

        string[] yanlislar = tersCevirildiMi ? sp.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item2, true) : sp.Rastgele4KelimeYaDaWordGetir(dogruCevap.Item1, false);
        //Eðer tersCevirildiMi switch'ine dokunulmadýysa ingilizce kelimeyi tahmin ediyoruz iþaretlendiðinde türkçe kelimeyi tahmin ediyoruz.

        int indexx = 0;
        soruS++;
        word.Text = tersCevirildiMi ? dogruCevap.Item1 : dogruCevap.Item2; // ing mi tr mi
        //label5.Text = soru + soruS;

        vy.gosterilenKelimelerDogru.Add(dogruCevap.Item2);

        // Rastgele bir RadioButton seçin
        randomIndex = random.Next(0, radioButtons.Length); // 0 ile (RadioButton dizisinin uzunluðu - 1) arasýnda rastgele bir indeks seçin

        for (int i = 0; i < radioButtons.Length; i++)
        {
            if (i != randomIndex)
            {
                radioButtons[i].Content = yanlislar[indexx]; ;//eðer seçilen rastgele button deðil ise diðer buttonlara deðer giriyorum rastgele.
                indexx++;
            }
            else
                radioButtons[i].Content = tersCevirildiMi ? dogruCevap.Item2 : dogruCevap.Item1;//belirlediðim cevabý atýyorum.
        }
        MetindenSese(word.Text);
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

    private void switchCevir(object sender, ToggledEventArgs e)
    {
        tersCevirildiMi = switchCevirr.On;
        switchCevirr.Text = tersCevirildiMi ? "Tr->Eng Geç" : "Eng->Tr Back";

        if(tersCevirildiktenSonraTiklandiMi)
        {
            sirala();
            tersCevirildiktenSonraTiklandiMi = false;
        }
            

        // Ýþaretlenip iþaretlenmediði ile ilgili iþlemleri burada yapabilirsiniz
    }

    private async void BaslatButton_Clicked(object sender,EventArgs e)
    {
        if (!buttonStarted)
        {
            foreach (var i in radioButtons)
                i.IsEnabled = true;
            SayfayiAc();
            buttonStarted = true;
            button.Text = "Finish";
        }
        else
        {
            int sorus = dogru + yanlis;
            if (sorus == 0)
            {
                await DisplayAlert("Hata", "Sayfaya yönlendirilmek için cevap veriniz.", "Tamam");
                return;
            }

            var result = await DisplayAlert("Listeye Ekle", "Sayfaya yönlendiriliyorsunuz", "Eklemeden çık", "Devam et");

            if (!result)
            {
                vy.trMii = tersCevirildiMi;//false ise ing true ise tr

                await Navigation.PushAsync(new SorguSayfasi());
                // Kullanýcý "Evet" butonuna týkladý
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