using System;
using System.Text.Json.Serialization;

namespace Frontend.Modelos
{
    public class Usuarios
    {
        [JsonPropertyName("cpf")]
        public string CPF { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}
