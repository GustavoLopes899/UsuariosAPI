using Frontend.JanelaPrincipal.VM;
using System.Windows;

namespace Frontend.JanelaPrincipal.View
{
    /// <summary>
    /// Interaction logic for JanelaPrincipal.xaml
    /// </summary>
    public partial class TelaJanelaPrincipal : Window
    {
        public TelaJanelaPrincipal(double altura = 600, double largura = 900)
        {
            this.InitializeComponent();
            this.Height = altura;
            this.Width = largura;
            this.DataContext = new JanelaPrincipalVM();
        }
    }
}
