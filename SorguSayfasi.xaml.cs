using System.Collections.ObjectModel;

namespace Kelimecim
{
    public partial class SorguSayfasi : ContentPage
    {
        public ObservableCollection<SwitchCellViewModel> PageItems { get; set; }

        public SorguSayfasi()
        {
            InitializeComponent();
            BindingContext = this;
            VeriListele();
        }

        private void VeriListele()
        {
             PageItems = new ObservableCollection<SwitchCellViewModel>();
           for(int i=0;i<4;i++) 
            {
                var yanlisSwitchCell = new SwitchCell { Text = $"Yanlis Switch {i}" };
                yanlisKelimeListesi.Root[0].Add(yanlisSwitchCell);
            }
            for (int i = 0; i < 5; i++)
            {
                var dogruSwitchCell = new SwitchCell { Text = $"Dogru Switch {i}" };
               dogruKelimeListesi.Root[0].Add(dogruSwitchCell);
            }
        }

        private async void button_Clicked(object sender, EventArgs e)
        {
                // Display success message after successful data save
                await DisplayAlert("Bilgi", "Doðru veriler kaydedildi!", "Tamam");
        }
    }
}

