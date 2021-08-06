using Frontend.Login.VM;
using System.Windows;

namespace Frontend.Login.View
{
    /// <summary>
    /// Interaction logic for TelaLogin.xaml
    /// </summary>
    public partial class TelaLogin : Window
    {
        public TelaLogin(double altura = 600, double largura = 500)
        {
            this.InitializeComponent();
            this.Height = altura;
            this.Width = largura;
            this.DataContext = new LoginVM();
        }
    }
}
