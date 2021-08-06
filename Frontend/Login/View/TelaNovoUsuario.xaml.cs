using Frontend.Login.VM;
using System.Windows;

namespace Frontend.Login.View
{
    /// <summary>
    /// Interaction logic for TelaNovoUsuario.xaml
    /// </summary>
    public partial class TelaNovoUsuario : Window
    {
        public TelaNovoUsuario()
        {
            this.InitializeComponent();
            this.DataContext = new NovoUsuarioVM();
        }
    }
}
