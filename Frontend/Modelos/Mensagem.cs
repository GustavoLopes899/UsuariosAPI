using System.Text.Json.Serialization;

namespace Frontend.Modelos
{
    public class Mensagem
    {
        [JsonPropertyName("mensagem")]
        public string Msg { get; set; }
    }
}
