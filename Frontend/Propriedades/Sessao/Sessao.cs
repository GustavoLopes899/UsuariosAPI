using Ferramentas;
using Frontend.Modelos;
using System.Net;
using System.Net.Http;

namespace Frontend.Program.Sessao
{
    public static class Sessao
    {
        private static readonly HttpClient httpClient;
        private static readonly string url;

        static Sessao()
        {
            httpClient = new HttpClient();
            ArquivoIni arquivoIni = new ArquivoIni();
            url = arquivoIni.Ler("url", "conexao");
            if (url.Contains("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
        }

        public static TokenSessao Token { get; set; }

        public static HttpClient Cliente => httpClient;

        public static string UrlAutenticacao => url + "/login/autenticar";

        public static string UrlAdicionarCadastrador => url + "/cadastrador/adicionar";
    }
}
