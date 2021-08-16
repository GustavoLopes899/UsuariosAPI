using Frontend.Login.VM;
using System.Windows;

namespace Frontend.Login.View
{
    /// <summary>
    /// Interaction logic for TelaNovoUsuario.xaml
    /// </summary>
    public partial class TelaNovoUsuario : Window
    {
        // TODO: Adicionar validação dos campos na View
        public TelaNovoUsuario()
        {
            this.InitializeComponent();
            this.DataContext = new NovoUsuarioVM();
        }

        private void SalvarNovoUsuario(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is NovoUsuarioVM novoUsuarioVM)
            {
                string usuario = this.tbUsuario?.Text;
                string senha = this.pbSenha?.Password;
                string confirmaSenha = this.pbConfirmaSenha?.Password;
                bool valido = novoUsuarioVM.AdicionaNovoUsuario(usuario, senha, confirmaSenha, out string mensagem);
                if (valido)
                {
                    _ = MessageBox.Show(mensagem, "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _ = MessageBox.Show(mensagem, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }
    }
}
