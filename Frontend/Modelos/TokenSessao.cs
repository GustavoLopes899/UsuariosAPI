using System.Text.Json.Serialization;

namespace Frontend.Modelos
{
    public class TokenSessao
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
