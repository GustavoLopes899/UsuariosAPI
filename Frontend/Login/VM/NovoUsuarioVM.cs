using Ferramentas;
using Frontend.Modelos;
using Frontend.Program.Sessao;
using Frontend.Propriedades.Util;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Frontend.Login.VM
{
    public class NovoUsuarioVM
    {
        public bool AdicionaNovoUsuario(string usuario, string senha, string confirmaSenha, out string mensagem)
        {
            if (this.ValidaNovoUsuario(usuario, senha, confirmaSenha, out mensagem))
            {
                this.SalvaNovoUsuario(usuario, senha, out mensagem);
                return true;
            }
            return false;
        }

        public bool ValidaNovoUsuario(string usuario, string senha, string confirmaSenha, out string mensagem)
        {
            if (usuario.Length >= Constantes.TamanhoMinimoNomeUsuario && this.ValidaNomeUsuario(usuario))
            {
                if (senha.Length >= Constantes.TamanhoMinimoSenha)
                {
                    if (senha.Equals(confirmaSenha))
                    {
                        mensagem = "Usuário adicionado com sucesso!";
                        return true;
                    }
                    else
                    {
                        mensagem = "As senhas inseridas não conferem.\nTente novamente...";
                    }
                }
                else
                {
                    mensagem = "A senha não respeita as regras de nome de usuário.\nTente novamente...";
                }
            }
            else
            {
                mensagem = "O usuário informado não respeita as regras de nome de usuário.\nTente novamente...";
            }
            return false;
        }

        private bool ValidaNomeUsuario(string usuario)
        {
            return Regex.IsMatch(usuario, @"^[a-zA-Z0-9]+$");
        }

        public void SalvaNovoUsuario(string usuario, string senha, out string mensagem)
        {
            IDictionary<string, object> dados = new Dictionary<string, object>
            {
                { "usuario", usuario },
                { "senha", senha },
                { "ativo", true },
                { "permissao", null }
            };
            try
            {
                string resposta = Requisicoes.Post(Sessao.UrlAdicionarCadastrador, dados, null);
                if (!string.IsNullOrEmpty(resposta))
                {
                    Mensagem mensagemAPI = JsonSerializer.Deserialize<Mensagem>(resposta);
                    mensagem = mensagemAPI.Msg;
                }
                else
                {
                    mensagem = "Erro na requisição";
                }
            }
            catch (Exception ex)
            {
                mensagem = Logger.TrataMensagemExcecao(ex);
                Logger.GravarLog(mensagem);
            }
        }
    }
}
