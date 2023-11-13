namespace Kelimecim;

public partial class CoktanSecmeli : ContentPage
{
    GoogleSheets gs = new GoogleSheets();
    private Random random = new Random();

    private RadioButton[] radioButtons; // System.Windows.Forms.RadioButton t�r�n� kullan�yoruz

    int randomIndex;
    int yanlis = 0;
    int dogru = 0;
    int soruS = 0;

    string yaziyanlis = "Yanl�� Say�s� = ";
    string yazidogru = "Do�ru Say�s� = ";
    //string soru = "Soru ";

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
        sirala();
        dogruSayisi.Text = yazidogru + dogru;
        yanlisSayisi.Text = yaziyanlis + yanlis;
        Thread.Sleep(3200);
        MetindenSese(word.Text);
    }

    private async void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton rb = (RadioButton)sender;//Hangi button'a t�kland���n� anlay�p i�lem yapmak i�in bir de�i�kene at�yorum.
        if (rb.IsChecked)
        {
            radioButtons[randomIndex].TextColor = Color.FromRgb(0, 255, 0);//do�ru olan cevab� ye�il i�aretliyorum.


            if (rb != radioButtons[randomIndex])
            {
                rb.TextColor = Color.FromRgb(255, 0, 0);//e�er do�ru cevap de�il ise k�rm�z� renkte olsun i�aretledi�im.
                yanlis++;
                yanlisSayisi.Text = yaziyanlis + yanlis;
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

        // Rastgele bir RadioButton se�in
        randomIndex = random.Next(0, radioButtons.Length); // 0 ile (RadioButton dizisinin uzunlu�u - 1) aras�nda rastgele bir indeks se�in

        for (int i = 0; i < radioButtons.Length; i++)
        {
            if (i != randomIndex)
            {
                Tuple<string, string> yanlisCevap = gs.RastgeleKelimeGetirVTOrMyList(true);
                radioButtons[i].Content = yanlisCevap.Item1;//e�er se�ilen rastgele button de�il ise di�er buttonlara de�er giriyorum rastgele.
            }
            else
                radioButtons[i].Content = dogruCevap.Item1;//belirledi�im cevab� at�yorum.
        }
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
}