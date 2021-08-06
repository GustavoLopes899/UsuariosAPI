using Frontend.Login.View;
using System.Windows;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Inicializacao(object sender, StartupEventArgs e)
        {
            TelaLogin login = new TelaLogin();
            login.Show();
        }
    }
}