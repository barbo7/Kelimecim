using System.Collections.ObjectModel;

namespace Kelimecim
{
    public partial class SorguSayfasi : ContentPage
    {
        GoogleSheets gs = new GoogleSheets();
        public ObservableCollection<string> PageItems { get; set; }
        public ObservableCollection<SwitchCell> YanlisSwitchCells { get; set; }
        public ObservableCollection<SwitchCell> DogruSwitchCells { get; set; }

        public SorguSayfasi()
        {
            InitializeComponent();
            BindingContext = this;

            PageItems = new ObservableCollection<string>
            {
                "Yanlýþ Bildiklerim", "Doðru Bildiklrim"
            };

            YanlisSwitchCells = new ObservableCollection<SwitchCell>();
            DogruSwitchCells = new ObservableCollection<SwitchCell>();
            VeriListele();
        }

        private void VeriListele()
        {
            List<string> yanlisKelimeler = gs.gosterilenKelimelerYanlis;
            List<string> dogruKelimeler = gs.gosterilenKelimelerDogru;

            for (int i=0;i< yanlisKelimeler.Count(); i++) 
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
            gs.gosterilenKelimelerYanlis.Clear();
            gs.gosterilenKelimelerDogru.Clear();
        }

        private async void KaydetButton_Clicked(object sender, EventArgs e)
        {
            foreach (var yanlisS in YanlisSwitchCells)
                if (yanlisS.On)
                    gs.gosterilenKelimelerYanlis.Add(yanlisS.Text);

            foreach (var dogruS in DogruSwitchCells)
                if (dogruS.On)
                    gs.gosterilenKelimelerDogru.Add(dogruS.Text);

                // Display success message after successful data save
                await DisplayAlert("Bilgi", "Doðru veriler kaydedildi!", "Tamam");
            
        }
    }
}

