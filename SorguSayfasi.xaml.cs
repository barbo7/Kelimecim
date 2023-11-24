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
            // �rnek olarak 5 adet SwitchCell ekleyelim
            for (int i = 1; i <= 5; i++)
            {
                var yanlisSwitchCell = new SwitchCell { Text = $"Yanlis Switch {i}" };
                yanlisKelimeListesi.Root[0].Add(yanlisSwitchCell);

            }
        }
        private async void button_Clicked(object sender, EventArgs e)
        {
            // Do�ru Verileri Kaydet butonuna t�kland���nda yap�lacak i�lemler
            await DisplayAlert("Bilgi", "Do�ru veriler kaydedildi!", "Tamam");
        }
    }
}
