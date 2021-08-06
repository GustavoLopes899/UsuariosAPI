using Frontend.Program.Sessao;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Frontend.Propriedades.Util
{
    public class Requisicoes
    {
        public static string Post(string url, IDictionary<string, object> body, IDictionary<string, string> query)
        {
            url = MontarUrlFormatada(url, query);
            Sessao.Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token?.Token);
            StringContent conteudo = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            HttpResponseMessage requisicao = Sessao.Cliente.PostAsync(url, conteudo).GetAwaiter().GetResult();
            return requisicao.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static string Get(string url, IDictionary<string, object> _, IDictionary<string, string> query = null)
        {
            if (query != null)
            {
                url = MontarUrlFormatada(url, query);
            }
            Sessao.Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token?.Token);
            HttpResponseMessage requisicao = Sessao.Cliente.GetAsync(url).GetAwaiter().GetResult();
            return requisicao.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static string Delete(string url, IDictionary<string, object> _, IDictionary<string, string> query = null)
        {
            if (query != null)
            {
                url = MontarUrlFormatada(url, query);
            }
            Sessao.Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token?.Token);
            HttpResponseMessage requisicao = Sessao.Cliente.DeleteAsync(url).GetAwaiter().GetResult();
            return requisicao.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        private static string MontarUrlFormatada(string url, IDictionary<string, string> query)
        {
            if (query != null)
            {
                url = string.Concat(url, '?');
                for (int i = 0; i < query.Count; i++)
                {
                    KeyValuePair<string, string> item = query.ElementAt(i);
                    url = string.Concat(url, item.Key + '=');
                    url = string.Concat(url, item.Value.ToString());
                    if (i != query.Count - 1)
                    {
                        url = string.Concat(url, '&');
                    }
                }
            }
            return url;
        }
    }
}
