using Frontend.JanelaPrincipal.View;
using Frontend.JanelaPrincipal.VM;
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

        private void FazerLogin(object sender, RoutedEventArgs e)
        {
            string usuario = this.tbUsuario?.Text;
            string senha = this.pbSenha?.Password;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                _ = MessageBox.Show("Usuário e/ou senha não podem estar em branco!", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (this.DataContext is LoginVM loginVM)
            {
                if (loginVM.Autentica(usuario, senha))
                {
                    TelaJanelaPrincipal janelaPrincipal = new TelaJanelaPrincipal
                    {
                        DataContext = new JanelaPrincipalVM()
                    };
                    janelaPrincipal.Show();
                    this.Close();
                }
                else
                {
                    _ = MessageBox.Show("Login não autorizado! Usuário e/ou senha incorretos!", "Login inválido!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AbrirJanelaNovoUsuario(object sender, RoutedEventArgs e)
        {
            TelaNovoUsuario novoUsuario = new TelaNovoUsuario();
            _ = novoUsuario.ShowDialog();
        }
    }
}
