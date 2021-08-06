using Ferramentas;
using Frontend.Modelos;
using Frontend.Program.Sessao;
using Frontend.Propriedades.Util;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;

namespace Frontend.Login.VM
{
    public class LoginVM
    {
        public bool Autentica(string usuario, string senha)
        {
            string senhaCriptografada = MD5Hash.CalculaHash(senha);
            IDictionary<string, object> dados = new Dictionary<string, object>
            {
                { "usuario", usuario },
                { "senha", senhaCriptografada }
            };
            try
            {
                string resposta = Requisicoes.Post(Sessao.UrlAutenticacao, dados, null);
                if (!string.IsNullOrEmpty(resposta))
                {
                    TokenSessao token = JsonSerializer.Deserialize<TokenSessao>(resposta);
                    if (token.Token != null)
                    {
                        Sessao.Token = token;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Erro ao efetuar o login. Verifique o log", "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
                string mensagem = Logger.TrataMensagemExcecao(ex);
                Logger.GravarLog(mensagem);
            }
            return false;
        }
    }
}