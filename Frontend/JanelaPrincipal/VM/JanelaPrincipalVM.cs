using Ferramentas;
using Frontend.Modelos;
using Frontend.Program.Sessao;
using Frontend.Propriedades.Util;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Frontend.JanelaPrincipal.VM
{
    public class JanelaPrincipalVM : Notificacao
    {
        private ICommand buscar;
        private ICommand salvar;
        private ICommand adicionar;
        private ICommand editar;
        private ICommand deletar;
        private ICommand cancelar;
        private ICommand limpar;

        public JanelaPrincipalVM()
        {
            this.EmEdicao = false;
            this.NovoUsuario = false;
            this.CriarComandos();
        }

        private void CriarComandos()
        {
            this.buscar = new RelayCommand(
                obj =>
                {
                    this.BuscarUsuario(obj as string);
                });
            this.salvar = new RelayCommand(
                _ =>
                {
                    this.SalvarUsuario();
                });
            this.adicionar = new RelayCommand(
                _ =>
                {
                    this.AdicionarUsuario();
                });
            this.editar = new RelayCommand(
                _ =>
                {
                    this.EditarUsuario();
                });
            this.deletar = new RelayCommand(
                _ =>
                {
                    this.DeletarUsuario();
                });
            this.cancelar = new RelayCommand(
                _ =>
                {
                    this.Cancelar();
                });
            this.limpar = new RelayCommand(
                _ =>
                {
                    this.Limpar();
                });
        }

        private void BuscarUsuario(string cpf)
        {
            bool valido = Checagem.CPF(cpf);
            if (valido)
            {
                IDictionary<string, string> query = new Dictionary<string, string> { { "cpf", cpf } };
                this.Usuario = Deserializador.Deserializar<Usuarios>(Sessao.UrlUsuarioBuscar, null, query, Requisicoes.Get, out _);
                if (this.Usuario == null)
                {
                    MessageBox.Show("Usuário não encontrado!", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                this.Dependentes = Deserializador.Deserializar<IEnumerable<Usuarios>>(Sessao.UrlBuscarDependentes, null, query, Requisicoes.Get, out _);
                this.EmEdicao = false;
                this.NovoUsuario = false;
                this.NotificaTudo();
            }
            else
            {
                _ = MessageBox.Show("CPF inválido! Tente novamente", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SalvarUsuario()
        {
            Mensagem mensagemAPI;
            IDictionary<string, object> dados = new Dictionary<string, object>
                {
                    { "cpf", this.Usuario.CPF },
                    { "nome", this.Usuario.Nome },
                    { "ativo", this.Usuario.Ativo }
                };
            if (this.NovoUsuario)
            {
                if (Checagem.CPF(this.Usuario.CPF))
                {
                    Usuarios usuario = Deserializador.Deserializar<Usuarios>(Sessao.UrlUsuarioAdicionar, dados, null, Requisicoes.Post, out string resposta);
                    if (usuario.CPF != null)
                    {
                        _ = MessageBox.Show("Usuário adicionado com sucesso!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        mensagemAPI = JsonSerializer.Deserialize<Mensagem>(resposta);
                        _ = MessageBox.Show(mensagemAPI.Msg, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    this.NovoUsuario = false;
                }
                else
                {
                    _ = MessageBox.Show("CPF inválido, tente novamente...", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                IDictionary<string, string> query = new Dictionary<string, string>
                {
                    { "cpf", this.Usuario.CPF }
                };
                mensagemAPI = Deserializador.Deserializar<Mensagem>(Sessao.UrlUsuarioEditar, dados, query, Requisicoes.Post, out string _);
                _ = MessageBox.Show(mensagemAPI.Msg, "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.EmEdicao = false;
            this.NotificaTudo();
        }

        private void AdicionarUsuario()
        {
            this.EmEdicao = true;
            this.NovoUsuario = true;
            this.BuscaCPF = string.Empty;
            this.Usuario = new Usuarios
            {
                CPF = string.Empty,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now
            };
            this.NotificaTudo();
        }

        private void EditarUsuario()
        {
            this.EmEdicao = true;
            this.NotificaTudo();
        }

        private void DeletarUsuario()
        {
            IDictionary<string, string> query = new Dictionary<string, string>
            {
                { "cpf", this.Usuario.CPF }
            };
            Mensagem mensagem = Deserializador.Deserializar<Mensagem>(Sessao.UrlUsuarioRemover, null, query, Requisicoes.Delete, out string _);
            _ = MessageBox.Show(mensagem.Msg, "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Limpar();
        }

        private void Cancelar()
        {
            this.Limpar();
        }

        private void Limpar()
        {
            this.NovoUsuario = false;
            this.EmEdicao = false;
            this.Usuario = null;
            this.Dependentes = null;
            this.BuscaCPF = string.Empty;
            this.NotificaTudo();
        }

        #region Notify
        private void NotificaTudo()
        {
            this.Notifica(nameof(this.Usuario));
            this.Notifica(nameof(this.BuscaCPF));
            this.Notifica(nameof(this.Dependentes));
            this.Notifica(nameof(this.Usuario));
            this.Notifica(nameof(this.EmEdicao));
            this.Notifica(nameof(this.NovoUsuario));
            this.Notifica(nameof(this.PodeBuscar));
            this.Notifica(nameof(this.PodeAdicionar));
            this.Notifica(nameof(this.PodeSalvarCancelar));
            this.Notifica(nameof(this.PodeEditarDeletar));
        }
        #endregion
        public ICommand ComandoBuscar => this.buscar;

        public ICommand ComandoSalvar => this.salvar;

        public ICommand ComandoAdicionar => this.adicionar;

        public ICommand ComandoEditar => this.editar;

        public ICommand ComandoDeletar => this.deletar;

        public ICommand ComandoCancelar => this.cancelar;

        public ICommand ComandoLimpar => this.limpar;

        public Usuarios Usuario { get; set; }

        public IEnumerable<Usuarios> Dependentes { get; set; }

        public string BuscaCPF { get; set; }

        public bool EmEdicao { get; set; }

        public bool NovoUsuario { get; set; }

        public bool PodeAdicionar => !this.EmEdicao;

        public bool PodeBuscar => !this.EmEdicao;

        public bool PodeSalvarCancelar => this.EmEdicao || this.NovoUsuario;

        public bool PodeEditarDeletar => !this.EmEdicao && !this.NovoUsuario && this.Usuario != null;
    }
}
