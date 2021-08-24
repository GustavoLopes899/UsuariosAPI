using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Frontend.Propriedades.Util
{
    public class Deserializador
    {
        public static T Deserializar<T>(string url, IDictionary<string, object> dados, IDictionary<string, string> query,
            Func<string, IDictionary<string, object>, IDictionary<string, string>, string> funcao, out string resposta) where T : class
        {
            // TODO: Log em arquivo
            try
            {
                resposta = funcao(url, dados, query);
                return JsonSerializer.Deserialize<T>(resposta);
            }
            catch (Exception ex)
            {
                resposta = ex.Message;
                return null;
            }
        }
    }
}
