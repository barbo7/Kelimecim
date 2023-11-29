namespace Kelimecim
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        private void ExitButton_Clicked(object sender, EventArgs e)
        {
            // Uygulamadan çıkış yap
            System.Environment.Exit(0);
        }
        private async void AnaSayfayaDon_Clicked(object sender, EventArgs e)
        {
            string targetRoute = "MainPage"; // Çeviri sayfasının Route değeri
            
            await Shell.Current.GoToAsync($"//{targetRoute}");
        }
    }
}