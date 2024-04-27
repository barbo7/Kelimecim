using System.Collections.ObjectModel;

namespace Kelimecim
{
    public partial class SorguSayfasi : ContentPage
    {
        //GoogleSheets gs = GoogleSheets.Instance;
        SqliteProcess sp = SqliteProcess.Instance;
        VeriYonlendir vy = VeriYonlendir.Instance;
        public ObservableCollection<string> PageItems { get; set; }
        public ObservableCollection<SwitchCell> YanlisSwitchCells { get; set; }
        public ObservableCollection<SwitchCell> DogruSwitchCells { get; set; }

        public SorguSayfasi()
        {
            InitializeComponent();
            BindingContext = this;

            PageItems = new ObservableCollection<string>
            {
                "Yanlış Bildiklerim", "Doğru Bildiklrim"
            };

            YanlisSwitchCells = new ObservableCollection<SwitchCell>();
            DogruSwitchCells = new ObservableCollection<SwitchCell>();
            VeriListele();
        }

        private void VeriListele()
        {
            List<string> yanlisKelimeler = vy.gosterilenKelimelerYanlis;
            List<string> dogruKelimeler = vy.gosterilenKelimelerDogru;

            for (int i = 0; i < yanlisKelimeler.Count(); i++)
            {
                var yanlisSwitchCell = new SwitchCell { Text = yanlisKelimeler[i] };
                YanlisSwitchCells.Add(yanlisSwitchCell);
                yanlisKelimeListesi.Root[0].Add(yanlisSwitchCell);
            }
            for (int i = 0; i < dogruKelimeler.Count(); i++)
            {
                var dogruSwitchCell = new SwitchCell { Text = dogruKelimeler[i] };
                DogruSwitchCells.Add(dogruSwitchCell);
                dogruKelimeListesi.Root[0].Add(dogruSwitchCell);
            }
            vy.gosterilenKelimelerYanlis.Clear();
            vy.gosterilenKelimelerDogru.Clear();
        }

        private async void KaydetButton_Clicked(object sender, EventArgs e)
        {

            List<string> eklenecekVeriler = new();
            foreach (var yanlisS in YanlisSwitchCells)
                if (yanlisS.On)
                    eklenecekVeriler.Add(yanlisS.Text);

            foreach (var dogruS in DogruSwitchCells)
                if (dogruS.On)
                    eklenecekVeriler.Add(dogruS.Text);

            foreach (string i in eklenecekVeriler)
            {
                //gs.VeriEkle(i);// Böyle yapmak yerine database'de mevcut olan kelimenin karşılığını bulup ikisini birden eklemek daha mantıklı olur.
                string kelime = sp.KelimeAraENG(i);
                sp.VeriEkle(i, kelime);
            }
            if (eklenecekVeriler.Count < 1)
            {
                var result = await DisplayAlert("Uyarı", "Kaydetmeden çıkmak istiyor musunuz?", "Evet", "Hayır");
                if (result)
                {
                    await Navigation.PopAsync();
                }
                return;

            }
            // Display success message after successful data save
            await DisplayAlert("Bilgi", "İstediğiniz bilgiler kelime defterinize eklendi.", "Tamam");
            await Navigation.PopAsync();


        }
    }
}

