using Frontend.JanelaPrincipal.VM;
using System;
using System.Windows;
using System.Windows.Controls;

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
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botao)
            {
                JanelaPrincipalVM dataContext = botao.DataContext as JanelaPrincipalVM;
                dataContext.Usuario.CPF = this.tbCPF.Text;
                dataContext.Usuario.Nome = this.tbNome.Text;
                dataContext.Usuario.Ativo = this.cbAtivo.IsChecked.Value;
                if (dataContext.NovoUsuario)
                {
                    dataContext.Usuario.DataCriacao = DateTime.Now;
                    dataContext.Usuario.DataAlteracao = DateTime.Now;
                }
                else
                {
                    dataContext.Usuario.DataAlteracao = DateTime.Now;
                }
            }
        }

        private void Limpar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botao)
            {
                JanelaPrincipalVM dataContext = botao.DataContext as JanelaPrincipalVM;
                dataContext.Usuario = null;
                dataContext.Dependentes = null;
                this.mtbBusca.Text = string.Empty;
                this.tbCPF.Text = string.Empty;
                this.tbNome.Text = string.Empty;
                this.tbDataCriacao.Text = string.Empty;
                this.tbDataAlteracao.Text = string.Empty;
            }
        }
    }
}
