namespace Kelimecim
{
    public partial class SorguSayfasi : ContentPage
    {
        public SorguSayfasi()
        {
            InitializeComponent();
            VeriListele();
        }

        private void VeriListele()
        {
            // Örnek olarak 5 adet SwitchCell ekleyelim
            for (int i = 1; i <= 5; i++)
            {
                var yanlisSwitchCell = new SwitchCell { Text = $"Yanlis Switch {i}" };
                yanlisKelimeListesi.Root[0].Add(yanlisSwitchCell);

            }
        }
        private async void button_Clicked(object sender, EventArgs e)
        {
            // Doðru Verileri Kaydet butonuna týklandýðýnda yapýlacak iþlemler
            await DisplayAlert("Bilgi", "Doðru veriler kaydedildi!", "Tamam");
        }
    }
}
